using System.Threading;
using System.Threading.Tasks;

namespace OzonEdu.MerchandiseService.Domain.SeedWork
{
    public interface IRepository<TAggregateRoot>
    {
        Task<TAggregateRoot> AddAsync(TAggregateRoot itemToAdd, CancellationToken cancellationToken = default);
        Task<TAggregateRoot> UpdateAsync(TAggregateRoot itemToUpdate, CancellationToken cancellationToken = default);

        Task SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}