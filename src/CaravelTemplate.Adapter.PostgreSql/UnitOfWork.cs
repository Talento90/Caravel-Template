using CaravelTemplate.Application.Data;

namespace CaravelTemplate.Adapter.PostgreSql;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public UnitOfWork(ApplicationDbContext context, IBookRepository bookRepository)
    {
        BookRepository = bookRepository;
        _context = context;
    }

    public IBookRepository BookRepository { get; }
    
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}