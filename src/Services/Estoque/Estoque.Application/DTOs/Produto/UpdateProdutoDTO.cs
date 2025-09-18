namespace Estoque.Application.DTOs.Produto
{
    public record UpdateProdutoDTO(
        int Id,
        string Nome,
        string Descricao,
        decimal Preco,
        int QuantidadeEmEstoque);
}
