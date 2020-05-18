using System;
using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Entities;

namespace CaravelTemplate.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        IUnitOfWork UnitOfWork { get; }
        Task<T> GetAsync(Guid id, CancellationToken ct = default);
        void DeleteAsync(T entity);
        void UpdateAsync(T entity);
        Task CreateAsync(T entity, CancellationToken ct = default);
    }
}