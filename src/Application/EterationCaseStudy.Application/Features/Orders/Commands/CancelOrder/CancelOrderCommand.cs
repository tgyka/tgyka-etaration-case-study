using EterationCaseStudy.Domain.Repositories;
using MediatR;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Application.Features.Orders.Commands.CancelOrder
{
    public record CancelOrderCommand(int OrderId) : IRequest<bool>;

    public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IOrderRepository _orders;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;

        public CancelOrderHandler(IOrderRepository orders, IUnitOfWork uow, ICurrentUser currentUser)
        {
            _orders = orders; _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orders.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null) return false;

            if (!_currentUser.IsAdmin)
            {
                var uid = _currentUser.UserId ?? 0;
                if (order.UserId != uid) return false;
            }

            order.Cancel();
            _orders.Update(order);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
