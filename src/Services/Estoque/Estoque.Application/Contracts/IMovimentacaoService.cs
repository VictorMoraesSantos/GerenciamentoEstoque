using Estoque.Application.DTOs.Movimentacao;
using Estoque.Domain.Enums;
using Shared.Common;

namespace Estoque.Application.Contracts
{
    public interface IMovimentacaoService : IService<MovimentacaoDTO, CreateMovimentacaoDTO, UpdateMovimentacaoDTO, int>
    {
        Task<IEnumerable<MovimentacaoDTO>> GetByTipoAsync(
            TipoMovimentacao tipo,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<MovimentacaoDTO>> GetByPeriodoAsync(
            DateTime dataInicial,
            DateTime dataFinal,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<MovimentacaoDTO>> GetByProdutoIdAsync(
            int produtoId,
            CancellationToken cancellationToken = default);
    }
}
