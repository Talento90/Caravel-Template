using Caravel.Functional;
using CaravelTemplate.Application.Data;
using CaravelTemplate.Books;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Adapter.PostgreSql.Repositories;

public class BookRepository : IBookRepository
{
    public BookRepository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    private ApplicationDbContext DbContext { get; }


    public async Task<IEnumerable<Book>> GetBooksASync(int skip, int pagSize, CancellationToken ct)
    {
        var books = await DbContext.Books.Skip(skip).Take(pagSize).ToListAsync(ct);
        return books;
    }

    public async Task<Result<Book>> GetBookAsync(Guid id, CancellationToken ct)
    {
        var book = await DbContext.Books.FindAsync([id], cancellationToken: ct);

        return book is not null ? Result<Book>.Success(book) : Result<Book>.Failure(BookErrors.NotFound(id));
    }

    public async Task<Result<Book>> CreateBookAsync(Book book, CancellationToken ct)
    {
        await DbContext.Books.AddAsync(book, ct);
        
        return Result<Book>.Success(book);
    }

    public Task UpdateBookAsync(Book book, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBookAsync(Book book, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}