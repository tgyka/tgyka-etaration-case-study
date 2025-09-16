using EterationCaseStudy.Domain.Entities.Base;

namespace EterationCaseStudy.Domain.Entities
{
    public class Role : Entity
    {
        public int Id { get; set; }
        public string Name { get; private set; }

        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private Role() { }
        public Role(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
        }

        public void Rename(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name is required", nameof(name)) : name.Trim();
        }
    }
}
