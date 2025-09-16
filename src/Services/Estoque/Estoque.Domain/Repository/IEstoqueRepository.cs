using Estoque.Domain.Enitities;
using Shared.Common;

namespace Estoque.Domain.Repository
{
    public interface IEstoqueRepository : IRepository<Enitities.Estoque, int>
    {
    }
}
