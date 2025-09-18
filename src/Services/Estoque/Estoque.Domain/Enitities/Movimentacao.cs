using Estoque.Domain.Enums;
using Shared.Common;

namespace Estoque.Domain.Enitities
{
    public class Movimentacao : BaseEntity<int>
    {
        public int Quantidade { get; private set; }
        public TipoMovimentacao Tipo { get; private set; }
        public string Motivo { get; private set; }

        protected Movimentacao() { }

        public Movimentacao(
            int quantidade,
            TipoMovimentacao tipo,
            string motivo)
        {
            Quantidade = quantidade > 0 ? quantidade : throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));
            Tipo = tipo;
            Motivo = !string.IsNullOrWhiteSpace(motivo) ? motivo : throw new ArgumentException("O motivo da movimentação é obrigatório.", nameof(motivo));
        }
    }
}
