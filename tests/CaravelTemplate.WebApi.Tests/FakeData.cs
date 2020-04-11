using Bogus;
using CaravelTemplate.Entities;

namespace CaravelTemplate.WebApi.Tests
{
    public static class FakeData
    {
        public static readonly Faker<Book> BookFaker = new Faker<Book>()
            .StrictMode(true)
            .RuleFor(d => d.Id, f => f.Random.Guid())
            .RuleFor(i => i.CreatedAtUtc, f => f.Date.Past().ToUniversalTime())
            .RuleFor(i => i.UpdatedAtUtc, f => f.Date.Past().ToUniversalTime())
            .RuleFor(d => d.Name, f => f.Random.String2(20))
            .RuleFor(d => d.Description, f => f.Random.String2(50));
    }
}