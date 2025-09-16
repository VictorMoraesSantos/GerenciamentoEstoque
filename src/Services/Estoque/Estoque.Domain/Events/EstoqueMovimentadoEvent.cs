using Estoque.Domain.Enums;
using Shared.Common;
using System;

namespace Estoque.Domain.Events
{
    public class EstoqueMovimentadoEvent : DomainEvent
    {
        public int MovimentacaoId { get; }
        public int ProdutoId { get; }
        public int QuantidadeMovimentada { get; }
        public TipoMovimentacao TipoMovimentacao { get; }

        public EstoqueMovimentadoEvent(
            int movimentacaoId,
            int produtoId,
            int quantidadeMovimentada,
            TipoMovimentacao tipoMovimentacao)
            : base()
        {
            MovimentacaoId = movimentacaoId;
            ProdutoId = produtoId;
            QuantidadeMovimentada = quantidadeMovimentada;
            TipoMovimentacao = tipoMovimentacao;
        }
    }
}