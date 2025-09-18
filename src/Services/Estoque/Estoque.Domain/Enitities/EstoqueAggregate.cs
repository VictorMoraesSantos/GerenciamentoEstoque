using Estoque.Domain.Enums;
using Estoque.Domain.Events;
using Shared.Common;
using Shared.Exceptions;

namespace Estoque.Domain.Enitities
{
    public class EstoqueAggregate : BaseEntity<int>
    {
        public int ProdutoId { get; set; }
        public Produto Produto { get; private set; }
        private readonly List<Movimentacao> _movimentacoes = new();
        public IReadOnlyCollection<Movimentacao> Movimentacoes => _movimentacoes.AsReadOnly();

        private EstoqueAggregate() { }

        public EstoqueAggregate(int produtoId)
        {
            ProdutoId = produtoId;
        }

        public void AdicionarEstoque(int quantidade, string motivo)
        {
            if (quantidade <= 0)
                throw new BusinessRuleException("A quantidade para entrada de estoque deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new BusinessRuleException("O motivo da movimentação é obrigatório.");

            var movimentacao = new Movimentacao(
                quantidade,
                TipoMovimentacao.Entrada,
                motivo);

            _movimentacoes.Add(movimentacao);
            Produto.AtualizarEstoque(quantidade);

            movimentacao.AddDomainEvent(new EstoqueMovimentadoEvent(
                movimentacao.Id,
                ProdutoId,
                quantidade,
                TipoMovimentacao.Entrada));

            AddDomainEvent(new EstoqueAtualizadoEvent(ProdutoId, Produto.QuantidadeEmEstoque));
        }

        public void RemoverEstoque(int quantidade, string motivo)
        {
            if (quantidade <= 0)
                throw new BusinessRuleException("A quantidade para saída de estoque deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new BusinessRuleException("O motivo da movimentação é obrigatório.");

            if (quantidade > Produto.QuantidadeEmEstoque)
                throw new BusinessRuleException($"Quantidade insuficiente em estoque. Disponível: {Produto.QuantidadeEmEstoque}");

            var movimentacao = new Movimentacao(
                quantidade,
                TipoMovimentacao.Saida,
                motivo);

            _movimentacoes.Add(movimentacao);
            Produto.AtualizarEstoque(-quantidade);

            movimentacao.AddDomainEvent(new EstoqueMovimentadoEvent(
                movimentacao.Id,
                ProdutoId,
                -quantidade,
                TipoMovimentacao.Saida));

            AddDomainEvent(new EstoqueAtualizadoEvent(ProdutoId, Produto.QuantidadeEmEstoque));
        }

        public void AjustarEstoque(int novaQuantidade, string motivo)
        {
            if (novaQuantidade < 0)
                throw new BusinessRuleException("A quantidade para ajuste de estoque não pode ser negativa.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new BusinessRuleException("O motivo do ajuste é obrigatório.");

            int diferenca = novaQuantidade - Produto.QuantidadeEmEstoque;

            if (diferenca > 0)
            {
                AdicionarEstoque(diferenca, $"Ajuste de estoque: {motivo}");
            }
            else if (diferenca < 0)
            {
                RemoverEstoque(Math.Abs(diferenca), $"Ajuste de estoque: {motivo}");
            }
        }

        public bool EstoqueAbaixoDoMinimo(int quantidadeMinima)
        {
            return Produto.QuantidadeEmEstoque < quantidadeMinima;
        }

        public IEnumerable<Movimentacao> ObterHistoricoMovimentacoes(DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            var query = _movimentacoes.AsEnumerable();

            if (dataInicial.HasValue)
                query = query.Where(m => m.CreatedAt >= dataInicial.Value);

            if (dataFinal.HasValue)
                query = query.Where(m => m.CreatedAt <= dataFinal.Value);

            return query.OrderByDescending(m => m.CreatedAt).ToList();
        }
    }
}
