using Shared.Common;

namespace Estoque.Domain.Events
{
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