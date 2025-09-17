using Estoque.Domain.Enitities;
using Estoque.Domain.Repository;
using Estoque.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Estoque.Infrastructure.Persistence.Repositories
{
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly ApplicationDbContext _context;

        public MovimentacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movimentacao?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Movimentacoes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<Movimentacao?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Movimentacoes
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<Movimentacao?>> Find(Expression<Func<Movimentacao, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _context.Movimentacoes
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Create(Movimentacao entity, CancellationToken cancellationToken = default)
        {
            await _context.Movimentacoes.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Movimentacao entity, CancellationToken cancellationToken = default)
        {
            _context.Movimentacoes.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Movimentacao entity, CancellationToken cancellationToken = default)
        {
            _context.Movimentacoes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
