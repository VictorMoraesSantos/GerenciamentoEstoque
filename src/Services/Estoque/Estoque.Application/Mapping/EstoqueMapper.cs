using Estoque.Application.DTOs.Estoque;
using Estoque.Domain.Enitities;

namespace Estoque.Application.Mapping
{
    public static class EstoqueMapper
    {
        public static EstoqueDTO ToDTO(EstoqueAggregate entity)
        {
            var dto = new EstoqueDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                ProdutoMapper.ToDTO(entity.Produto),
                entity.Movimentacoes.Select(MovimentacaoMapper.ToDTO).ToList());

            return dto;
        }

        public static EstoqueAggregate ToEntity(CreateEstoqueDTO dto)
        {
            var entity = new EstoqueAggregate(dto.produtoId);

            return entity;
        }
    }
}
