using Estoque.Domain.Enums;

namespace Estoque.Application.DTOs.Movimentacao
{
    public record MovimentacaoDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int Quantidade,
        TipoMovimentacao Tipo,
        string Motivo);
}
