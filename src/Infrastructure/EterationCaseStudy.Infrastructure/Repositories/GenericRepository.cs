using System.Linq.Expressions;
using EterationCaseStudy.Domain.Entities.Base;
using EterationCaseStudy.Domain.Repositories;
using EterationCaseStudy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : Entity
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;
        private readonly ICurrentUser _currentUser;

        public GenericRepository(AppDbContext db, ICurrentUser currentUser)
        {
            _db = db;
            _set = db.Set<T>();
            _currentUser = currentUser;
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _set.AsQueryable();
            if (predicate != null)
                query = query.Where(predicate);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            entity.CreatedDate = now;
            entity.CreatedById = _currentUser.UserId;
            await _set.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedById = _currentUser.UserId;
            _set.Update(entity);
        }

        public void Remove(T entity)
        {
            if (entity is Entity e)
            {
                e.IsDeleted = true;
                e.ModifiedDate = DateTime.UtcNow;
                e.ModifiedById = _currentUser.UserId;
                _set.Update((T)e);
            }
            else
            {
                _set.Remove(entity);
            }
        }

        public IQueryable<T> Query() => _set.AsQueryable();
    }
}
