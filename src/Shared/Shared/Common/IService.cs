using System.Linq.Expressions;

namespace Shared.Common
{
    public interface IService<TDto, TCreateDto, TUpdateDto, TId>
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<TDto?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TDto>> FindAsync(Expression<Func<TDto, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);
        Task<TDto> UpdateAsync(TId id, TUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
        Task<PagedResult<TDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }

    public interface IService<TDto, TInputDto, TId> : IService<TDto, TInputDto, TInputDto, TId>
        where TDto : class
        where TInputDto : class
    {
    }

    public interface IService<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
