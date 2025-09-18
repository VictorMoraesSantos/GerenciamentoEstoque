using Estoque.Domain.Enums;

namespace Estoque.Application.DTOs.Movimentacao
{
    public record UpdateMovimentacaoDTO(
        int Id,
        int Quantidade,
        TipoMovimentacao Tipo,
        string Motivo);
}
