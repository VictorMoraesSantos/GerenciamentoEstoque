using Estoque.Domain.Enitities;
using Estoque.Domain.Repository;
using Estoque.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Estoque.Infrastructure.Persistence.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly ApplicationDbContext _context;

        public EstoqueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EstoqueAggregate?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Estoques
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<EstoqueAggregate?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Estoques
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<EstoqueAggregate?>> Find(Expression<Func<EstoqueAggregate, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _context.Estoques
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Create(EstoqueAggregate entity, CancellationToken cancellationToken = default)
        {
            await _context.Estoques.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(EstoqueAggregate entity, CancellationToken cancellationToken = default)
        {
            _context.Estoques.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(EstoqueAggregate entity, CancellationToken cancellationToken = default)
        {
            _context.Estoques.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
