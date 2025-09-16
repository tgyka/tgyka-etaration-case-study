using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using MediatR;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Application.Features.Orders.Queries
{
    public record GetOrdersQuery() : IRequest<IReadOnlyList<Order>>;

    public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IReadOnlyList<Order>>
    {
        private readonly IOrderRepository _orders;
        private readonly ICurrentUser _currentUser;

        public GetOrdersHandler(IOrderRepository orders, ICurrentUser currentUser)
        {
            _orders = orders;
            _currentUser = currentUser;
        }

        public Task<IReadOnlyList<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _orders.Query();
            if (!_currentUser.IsAdmin)
            {
                var uid = _currentUser.UserId ?? 0;
                query = query.Where(o => o.UserId == uid);
            }
            var list = query.ToList();
            return Task.FromResult((IReadOnlyList<Order>)list);
        }
    }
}
