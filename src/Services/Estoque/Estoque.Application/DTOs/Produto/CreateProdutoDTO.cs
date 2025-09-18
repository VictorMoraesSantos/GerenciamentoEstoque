namespace Estoque.Application.DTOs.Produto
{
    public record CreateProdutoDTO(
        string Nome,
        string Descricao,
        decimal Preco,
        int QuantidadeEmEstoque);
}
