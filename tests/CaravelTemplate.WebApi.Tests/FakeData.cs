using Bogus;
using CaravelTemplate.Entities;

namespace CaravelTemplate.WebApi.Tests
{
    public static class FakeData
    {
        public static Faker<Book> BookFaker() => new Faker<Book>()
            .StrictMode(false)
            .RuleFor(d => d.Id, f => f.Random.Guid())
            .RuleFor(i => i.Created, f => f.Date.Past().ToUniversalTime())
            .RuleFor(i => i.Updated, f => f.Date.Past().ToUniversalTime())
            .RuleFor(d => d.Name, f => f.Random.String2(20))
            .RuleFor(d => d.Description, f => f.Random.String2(50));
    }
}