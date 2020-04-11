using System;

namespace CaravelTemplate.Entities
{
    public class Book : Entity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}