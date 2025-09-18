using Estoque.Application.DTOs.Movimentacao;
using Estoque.Application.DTOs.Produto;

namespace Estoque.Application.DTOs.Estoque
{
    public record EstoqueDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        ProdutoDTO Produto,
        IEnumerable<MovimentacaoDTO> Movimentacoes);
}
