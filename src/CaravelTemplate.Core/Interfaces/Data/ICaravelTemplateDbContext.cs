using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Core.Interfaces.Data
{
    public interface ICaravelTemplateDbContext
    {
        public DbSet<Book> Books { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}