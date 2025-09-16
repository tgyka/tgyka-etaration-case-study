using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using EterationCaseStudy.Infrastructure.Persistence;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext db, ICurrentUser currentUser) : base(db, currentUser)
        {
        }
    }
}

