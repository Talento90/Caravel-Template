using Caravel.Entities;

namespace CaravelTemplate.Books
{
    public class Book : AggregateRoot
    {
        public string Name { get; init; }
        public string Description { get; init; }

        public Book(string name, string description) : base(new List<IDomainEvent>())
        {
            Name = name;
            Description = description;
        }
    }
}