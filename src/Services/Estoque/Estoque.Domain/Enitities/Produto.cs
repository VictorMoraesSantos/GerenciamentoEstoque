using Shared.Common;
using Shared.Exceptions;

namespace Estoque.Domain.Enitities
{
    public class Produto : BaseEntity<int>
    {
        public string Nome { get; private set; } = default!;
        public string Descricao { get; private set; } = default!;
        public decimal Preco { get; private set; }
        public int QuantidadeEmEstoque { get; private set; }
        public bool Ativo { get; private set; }

        public Produto(string nome, string descricao, decimal preco, int quantidadeEmEstoque, bool ativo)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            QuantidadeEmEstoque = quantidadeEmEstoque;
            Ativo = ativo;
        }

        public void AtualizarEstoque(int quantidade)
        {
            if (quantidade < 0 && Math.Abs(quantidade) > QuantidadeEmEstoque)
                throw new NotFoundException(Nome, Id);

            QuantidadeEmEstoque += quantidade;
            MarkAsUpdated();
        }

        public void AtualizarInformacoes(string nome, string descricao, decimal preco)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            MarkAsUpdated();
        }

        public void Ativar() => Ativo = true;
        public void Desativar() => Ativo = false;
    }
}
