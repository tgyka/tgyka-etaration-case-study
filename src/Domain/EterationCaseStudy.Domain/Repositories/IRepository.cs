using System.Linq.Expressions;
using EterationCaseStudy.Domain.Entities.Base;

namespace EterationCaseStudy.Domain.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Remove(T entity);
        IQueryable<T> Query();
    }
}