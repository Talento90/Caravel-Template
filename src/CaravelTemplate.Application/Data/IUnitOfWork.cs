namespace CaravelTemplate.Application.Data;

public interface IUnitOfWork
{
    IBookRepository BookRepository { get; }
    Task SaveChangesAsync(CancellationToken ct);
}