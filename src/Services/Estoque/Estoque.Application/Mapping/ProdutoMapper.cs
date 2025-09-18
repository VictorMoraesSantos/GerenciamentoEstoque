using Estoque.Application.DTOs.Produto;
using Estoque.Domain.Enitities;

namespace Estoque.Application.Mapping
{
    public static class ProdutoMapper
    {
        public static ProdutoDTO ToDTO(Produto entity)
        {
            var dto = new ProdutoDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Nome,
                entity.Descricao,
                entity.Preco,
                entity.QuantidadeEmEstoque);

            return dto;
        }
        public static Produto ToEntity(UpdateProdutoDTO dto)
        {
            var entity = new Produto(
                dto.Nome,
                dto.Descricao,
                dto.Preco,
                dto.QuantidadeEmEstoque,
                true);

            return entity;
        }
    }
}
