using Estoque.Application.DTOs.Movimentacao;
using Estoque.Domain.Enitities;

namespace Estoque.Application.Mapping
{
    public static class MovimentacaoMapper
    {
        public static MovimentacaoDTO ToDTO(Movimentacao entity)
        {
            var dto = new MovimentacaoDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Quantidade,
                entity.Tipo,
                entity.Motivo);

            return dto;
        }

        public static Movimentacao ToEntity(UpdateMovimentacaoDTO dto)
        {
            var entity = new Movimentacao(
                dto.Quantidade,
                dto.Tipo,
                dto.Motivo);

            return entity;
        }
    }
}
