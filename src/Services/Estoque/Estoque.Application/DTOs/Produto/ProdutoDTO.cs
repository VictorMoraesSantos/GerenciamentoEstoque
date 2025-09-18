namespace Estoque.Application.DTOs.Produto
{
    public record ProdutoDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Nome,
        string Descricao,
        decimal Preco,
        int QuantidadeEmEstoque);
}
