using Caravel.Entities;
using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Identity
{
    public class User : IdentityUser<Guid>, IAggregateRoot, IAuditable
    {
        private readonly List<IDomainEvent> _events = new();
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public void AddEvent(IDomainEvent domainEvent)
        {
            _events.Add(domainEvent);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public IEnumerable<IDomainEvent> Events => _events.AsReadOnly();
    }
}