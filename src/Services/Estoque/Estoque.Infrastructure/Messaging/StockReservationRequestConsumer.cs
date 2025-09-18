using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Messages;
using Estoque.Application.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Estoque.Infrastructure.Messaging
{
    public class StockReservationRequestConsumer : BaseConsumer<StockReservationRequested>
    {
        public StockReservationRequestConsumer(
            IEventBus eventBus,
            IServiceProvider serviceProvider,
            ILogger<StockReservationRequestConsumer> logger)
            : base(eventBus, serviceProvider, logger, "stock.reservation.requested")
        {
        }

        protected override async Task HandleMessage(StockReservationRequested message)
        {
            using var scope = ServiceProvider.CreateScope();
            var estoqueService = scope.ServiceProvider.GetRequiredService<IEstoqueService>();
            
            Logger.LogInformation("Processing stock reservation request for order: {OrderId}", message.OrderId);

            var result = new StockReservationResult
            {
                OrderId = message.OrderId,
                Success = true
            };

            foreach (var item in message.Items)
            {
                var estoqueDto = await estoqueService.GetByProdutoIdAsync(item.ProductId);
                if (estoqueDto == null)
                {
                    result.Success = false;
                    result.Items.Add(new StockReservationItemResult
                    {
                        ProductId = item.ProductId,
                        Reserved = false,
                        ReasonForFailure = "Produto não encontrado no estoque",
                        RequestedQuantity = item.Quantity,
                        AvailableQuantity = 0
                    });
                    continue;
                }

                try
                {
                    // Check if enough stock is available
                    if (estoqueDto.Produto.QuantidadeEmEstoque >= item.Quantity)
                    {
                        // Reserve stock (temporarily - we'll confirm later)
                        await estoqueService.RemoverEstoqueAsync(
                            estoqueDto.Id,
                            item.Quantity,
                            $"Reserva para pedido {message.OrderId}");

                        result.Items.Add(new StockReservationItemResult
                        {
                            ProductId = item.ProductId,
                            Reserved = true,
                            RequestedQuantity = item.Quantity,
                            AvailableQuantity = estoqueDto.Produto.QuantidadeEmEstoque - item.Quantity
                        });
                    }
                    else
                    {
                        result.Success = false;
                        result.Items.Add(new StockReservationItemResult
                        {
                            ProductId = item.ProductId,
                            Reserved = false,
                            ReasonForFailure = "Estoque insuficiente",
                            RequestedQuantity = item.Quantity,
                            AvailableQuantity = estoqueDto.Produto.QuantidadeEmEstoque
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error reserving stock for product {ProductId}", item.ProductId);
                    result.Success = false;
                    result.Items.Add(new StockReservationItemResult
                    {
                        ProductId = item.ProductId,
                        Reserved = false,
                        ReasonForFailure = "Erro ao processar reserva: " + ex.Message,
                        RequestedQuantity = item.Quantity,
                        AvailableQuantity = estoqueDto.Produto.QuantidadeEmEstoque
                    });
                }
            }

            // Send result back to Vendas service
            EventBus.Publish("stock.reservation.result", result);
        }
    }
}