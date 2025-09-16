using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using EterationCaseStudy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext db, ICurrentUser currentUser) : base(db, currentUser)
        {
        }

        public async Task<Order?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
    }
}
