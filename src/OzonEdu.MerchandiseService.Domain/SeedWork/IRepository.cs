using System.Threading;
using System.Threading.Tasks;

namespace OzonEdu.MerchandiseService.Domain.SeedWork
{
    public interface IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        Task<TAggregateRoot> AddAsync(TAggregateRoot itemToAdd, CancellationToken cancellationToken = default);
        Task<TAggregateRoot> UpdateAsync(TAggregateRoot itemToUpdate, CancellationToken cancellationToken = default);
    }
}