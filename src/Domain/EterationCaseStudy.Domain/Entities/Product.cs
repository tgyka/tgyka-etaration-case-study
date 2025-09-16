using EterationCaseStudy.Domain.Entities.Base;

namespace EterationCaseStudy.Domain.Entities
{
    public class Product : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Product() { }

        public Product(string name, decimal price, int stockQuantity, string description = null)
        {
            UpdateDetails(name, description);
            UpdatePrice(price);
            SetStock(stockQuantity);
        }

        public void UpdateDetails(string name,string description)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
            Description = description?.Trim();
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
            Price = price;
        }

        public void SetStock(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            StockQuantity = amount;
        }
    }
}
