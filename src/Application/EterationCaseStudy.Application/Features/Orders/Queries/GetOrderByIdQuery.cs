using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using MediatR;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Application.Features.Orders.Queries
{
    public record GetOrderByIdQuery(int Id) : IRequest<Order?>;

    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order?>
    {
        private readonly IOrderRepository _orders;
        private readonly ICurrentUser _currentUser;

        public GetOrderByIdHandler(IOrderRepository orders, ICurrentUser currentUser)
        {
            _orders = orders;
            _currentUser = currentUser;
        }

        public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orders.GetByIdAsync(request.Id, cancellationToken);
            if (order == null) return null;
            if (!_currentUser.IsAdmin)
            {
                var uid = _currentUser.UserId ?? 0;
                if (order.UserId != uid) return null;
            }
            return order;
        }
    }
}
