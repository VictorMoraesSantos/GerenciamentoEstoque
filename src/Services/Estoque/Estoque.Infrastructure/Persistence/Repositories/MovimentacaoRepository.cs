using Estoque.Domain.Enitities;
using Estoque.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Infrastructure.Persistence.Repositories
{
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        public Task Create(Movimentacao entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Movimentacao entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Movimentacao?>> Find(Expression<Func<Movimentacao, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Movimentacao?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Movimentacao?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(Movimentacao entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
