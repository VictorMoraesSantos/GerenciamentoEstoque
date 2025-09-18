using Estoque.Application.DTOs.Produto;
using Shared.Common;

namespace Estoque.Application.Contracts
{
    public interface IProdutoService : IService<ProdutoDTO, CreateProdutoDTO, UpdateProdutoDTO, int>
    {
        Task<ProdutoDTO> AtualizarInformacoesAsync(
            int id,
            string nome,
            string descricao,
            decimal preco,
            CancellationToken cancellationToken = default);
        Task<ProdutoDTO> AtivarAsync(int id, CancellationToken cancellationToken = default);
        Task<ProdutoDTO> DesativarAsync(int id, CancellationToken cancellationToken = default);
    }
}
