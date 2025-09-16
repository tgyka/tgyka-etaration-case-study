using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using MediatR;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderItemDto(int ProductId, int Quantity);
    public record CreateOrderCommand(int UserId, IReadOnlyList<CreateOrderItemDto> Items) : IRequest<int>;

    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orders;
        private readonly IProductRepository _products;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;

        public CreateOrderHandler(IOrderRepository orders, IProductRepository products, IUnitOfWork uow, ICurrentUser currentUser)
        {
            _orders = orders; _products = products;
            _uow = uow; _currentUser = currentUser;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin)
            {
                var uid = _currentUser.UserId ?? 0;
                if (request.UserId != uid) throw new UnauthorizedAccessException();
            }

            var order = new Order(request.UserId);
            foreach (var item in request.Items)
            {
                var product = await _products.GetByIdAsync(item.ProductId, cancellationToken) ?? throw new InvalidOperationException($"Product {item.ProductId} not found");
                if (!product.IsActive) throw new InvalidOperationException("Product inactive");
                if (product.StockQuantity < item.Quantity) throw new InvalidOperationException("Insufficient stock");
                product.SetStock(item.Quantity);
                order.AddItem(product, item.Quantity);
            }

            await _orders.AddAsync(order, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}
