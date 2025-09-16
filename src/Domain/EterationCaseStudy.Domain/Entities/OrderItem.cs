using EterationCaseStudy.Domain.Entities.Base;

namespace EterationCaseStudy.Domain.Entities
{
    public class OrderItem : Entity
    {
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal Subtotal => UnitPrice * Quantity;

        private OrderItem() { }

        public OrderItem(int productId, string productName, decimal unitPrice, int quantity)
        {
            if (productId <= 0) throw new ArgumentException("Invalid product id", nameof(productId));
            if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("Product name is required", nameof(productName));
            if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Price cannot be negative");
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive");

            ProductId = productId;
            ProductName = productName.Trim();
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public void IncreaseQuantity(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            Quantity += amount;
        }

        public void UpdateUnitPrice(decimal newPrice)
        {
            if (newPrice < 0) throw new ArgumentOutOfRangeException(nameof(newPrice));
            UnitPrice = newPrice;
        }
    }
}