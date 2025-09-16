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
    public class EstoqueRepository : IEstoqueRepository
    {
        public Task Create(Domain.Enitities.Estoque entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Domain.Enitities.Estoque entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Enitities.Estoque?>> Find(Expression<Func<Domain.Enitities.Estoque, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Enitities.Estoque?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Enitities.Estoque?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(Domain.Enitities.Estoque entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
