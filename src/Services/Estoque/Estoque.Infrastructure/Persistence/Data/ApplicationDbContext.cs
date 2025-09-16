using Estoque.Domain.Enitities;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EstoqueAggregate> Estoques { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Movimentacao> MovimentacoesEstoque { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
