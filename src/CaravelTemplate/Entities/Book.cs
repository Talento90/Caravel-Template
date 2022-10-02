using System.Collections.Generic;
using Caravel.Entities;

namespace CaravelTemplate.Entities
{
    public class Book : AggregateRoot
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public Book() : base(new List<IDomainEvent>())
        {
        }
    }
}