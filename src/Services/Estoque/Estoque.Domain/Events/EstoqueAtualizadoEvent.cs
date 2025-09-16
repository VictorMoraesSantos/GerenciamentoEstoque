using Shared.Common;

namespace Estoque.Domain.Events
{
    /// <summary>
    /// Event raised when a product's inventory level changes
    /// </summary>
    public class EstoqueAtualizadoEvent : DomainEvent
    {
        public int ProdutoId { get; }
        public int NovaQuantidade { get; }

        public EstoqueAtualizadoEvent(int produtoId, int novaQuantidade)
        {
            ProdutoId = produtoId;
            NovaQuantidade = novaQuantidade;
        }
    }
}