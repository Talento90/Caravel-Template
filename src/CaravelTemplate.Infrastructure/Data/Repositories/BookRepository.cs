using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Entities;
using CaravelTemplate.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Infrastructure.Data.Repositories;

public class BookRepository : IBookRepository
{
    private readonly CaravelTemplateTemplateDbContext _dbContext;

    public BookRepository(CaravelTemplateTemplateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Book>> GetBooks(int skip, int pagSize, CancellationToken ct)
    {
        return await _dbContext.Books
            .Skip(skip)
            .Take(pagSize)
            .ToListAsync(ct);
    }

    public async Task<Book?> GetBook(Guid id, CancellationToken ct)
    {
        return await _dbContext.Books.SingleOrDefaultAsync(b => b.Id == id, ct);
    }

    public Task UpdateBook(Book book, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteBook(Book book, CancellationToken ct)
    {
        _dbContext.Books.Remove(book);

        await Task.CompletedTask;
    }

    public async Task CreateBook(Book book, CancellationToken ct)
    {
        await _dbContext.Books.AddAsync(book, ct);
    }
}