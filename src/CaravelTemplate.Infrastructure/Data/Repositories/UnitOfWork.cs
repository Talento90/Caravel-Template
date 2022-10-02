using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Repositories;

namespace CaravelTemplate.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CaravelTemplateTemplateDbContext _dbContext;
    public IBookRepository BookRepository { get; }

    public UnitOfWork(CaravelTemplateTemplateDbContext dbContext)
    {
        _dbContext = dbContext;
        BookRepository = new BookRepository(dbContext);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}