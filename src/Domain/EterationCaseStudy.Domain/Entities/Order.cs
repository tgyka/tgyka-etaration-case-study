using EterationCaseStudy.Domain.Entities.Base;
using EterationCaseStudy.Domain.Enums;

namespace EterationCaseStudy.Domain.Entities
{
    public class Order : Entity
    {
        public string OrderNumber { get; private set; }
        public int UserId { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public DateTime OrderDate { get; private set; } = DateTime.UtcNow;

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal TotalAmount => _items.Sum(i => i.Subtotal);

        private Order() { }

        public Order(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user id", nameof(userId));
            UserId = userId;
            OrderNumber = $"ORD-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            Status = OrderStatus.Pending;
            OrderDate = DateTime.UtcNow;
        }

        public void AddItem(Product product, int quantity)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            EnsureModifiable();

            var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existing is null)
            {
                _items.Add(new OrderItem(product.Id, product.Name, product.Price, quantity));
            }
            else
            {
                existing.IncreaseQuantity(quantity);
            }
        }

        public void RemoveItem(int productId)
        {
            EnsureModifiable();
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return;
            _items.Remove(item);
        }

        public void Confirm()
        {
            EnsureModifiable();
            if (!_items.Any()) throw new InvalidOperationException("Order must have at least one item to confirm");
            Status = OrderStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel after shipment");
            Status = OrderStatus.Cancelled;
        }

        private void EnsureModifiable()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Order can only be modified while pending");
        }
    }
}
