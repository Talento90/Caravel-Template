using System;

namespace CaravelTemplate.Core.Books
{
    public class BookModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string Name { get; set; }  = null!;
        public string? Description { get; set; }
    }
}