using Caravel.Functional;
using CaravelTemplate.Books;

namespace CaravelTemplate.Application.Data;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetBooksASync(int skip, int pagSize, CancellationToken ct);
    Task<Result<Book>> GetBookAsync(Guid id, CancellationToken ct);
    Task<Result<Book>> CreateBookAsync(Book book, CancellationToken ct);
    Task UpdateBookAsync(Book book, CancellationToken ct);
    Task DeleteBookAsync(Book book, CancellationToken ct);
}