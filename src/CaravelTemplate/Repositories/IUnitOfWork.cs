using System.Threading;
using System.Threading.Tasks;

namespace CaravelTemplate.Repositories;

public interface IUnitOfWork
{
    IBookRepository BookRepository { get; }
    Task SaveChangesAsync(CancellationToken ct);
}