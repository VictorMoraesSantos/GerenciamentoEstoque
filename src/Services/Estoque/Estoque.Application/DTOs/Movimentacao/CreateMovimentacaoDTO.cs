using Estoque.Domain.Enums;

namespace Estoque.Application.DTOs.Movimentacao
{
    public record CreateMovimentacaoDTO(
        int Quantidade,
        TipoMovimentacao Tipo,
        string Motivo);
}
