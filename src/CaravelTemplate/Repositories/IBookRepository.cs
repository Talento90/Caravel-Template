using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Entities;

namespace CaravelTemplate.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetBooks(int skip, int pagSize, CancellationToken ct);
    Task<Book?> GetBook(Guid id, CancellationToken ct);
    Task CreateBook(Book book, CancellationToken ct);
    Task UpdateBook(Book book, CancellationToken ct);
    Task DeleteBook(Book book, CancellationToken ct);
}