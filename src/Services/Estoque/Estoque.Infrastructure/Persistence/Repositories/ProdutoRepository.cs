using Estoque.Domain.Enitities;
using Estoque.Domain.Repository;
using Estoque.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Estoque.Infrastructure.Persistence.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Produto?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Produtos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<Produto?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Produtos
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<Produto?>> Find(Expression<Func<Produto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _context.Produtos
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Create(Produto entity, CancellationToken cancellationToken = default)
        {
            await _context.Produtos.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Produto entity, CancellationToken cancellationToken = default)
        {
            _context.Produtos.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Produto entity, CancellationToken cancellationToken = default)
        {
            _context.Produtos.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
