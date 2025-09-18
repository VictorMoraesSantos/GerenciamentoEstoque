using Estoque.Application.DTOs.Estoque;
using Estoque.Application.DTOs.Movimentacao;
using Shared.Common;

namespace Estoque.Application.Contracts
{
    public interface IEstoqueService : IService<EstoqueDTO, CreateEstoqueDTO, UpdateEstoqueDTO, int>
    {
        Task<EstoqueDTO?> GetByProdutoIdAsync(int produtoId, CancellationToken cancellationToken = default);
        Task<EstoqueDTO> AdicionarEstoqueAsync(int estoqueId, int quantidade, string motivo, CancellationToken cancellationToken = default);
        Task<EstoqueDTO> RemoverEstoqueAsync(int estoqueId, int quantidade, string motivo, CancellationToken cancellationToken = default);
        Task<EstoqueDTO> AjustarEstoqueAsync(int estoqueId, int novaQuantidade, string motivo, CancellationToken cancellationToken = default);
        Task<bool> VerificarEstoqueAbaixoDoMinimoAsync(int estoqueId, int quantidadeMinima, CancellationToken cancellationToken = default);
        Task<IEnumerable<MovimentacaoDTO>> ObterHistoricoMovimentacoesAsync(
            int estoqueId,
            DateTime? dataInicial = null,
            DateTime? dataFinal = null,
            CancellationToken cancellationToken = default);
    }
}
