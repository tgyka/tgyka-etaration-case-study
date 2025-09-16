using EterationCaseStudy.Domain.Entities.Base;

namespace EterationCaseStudy.Domain.Entities
{
    public class User : Entity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public string PasswordHash { get; private set; }
        public int RoleId { get; private set; }
        public Role Role { get; private set; }

        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private User() { }

        public User(string username, string email, string fullName)
        {
            UpdateProfile(username, email, fullName);
        }

        public void UpdateProfile(string username, string email, string fullName)
        {
            Username = string.IsNullOrWhiteSpace(username) ? throw new ArgumentException("Username is required", nameof(username)) : username.Trim();
            Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email is required", nameof(email)) : email.Trim();
            FullName = string.IsNullOrWhiteSpace(fullName) ? throw new ArgumentException("Full name is required", nameof(fullName)) : fullName.Trim();
        }

        public Order CreateOrder()
        {
            var order = new Order(Id);
            _orders.Add(order);
            return order;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required", nameof(passwordHash));
            PasswordHash = passwordHash;
        }

        public void SetRole(Role role)
        {
            Role = role ?? throw new ArgumentNullException(nameof(role));
            RoleId = role.Id;
        }
    }
}
