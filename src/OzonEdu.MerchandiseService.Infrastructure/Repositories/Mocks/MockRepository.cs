using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Mocks
{
    public abstract class MockRepository<TAggregateRoot> where TAggregateRoot : Entity
    {
        protected readonly List<TAggregateRoot> MockSet = new List<TAggregateRoot>();
        private static int _identityCounter = 0;
        protected virtual bool AutoIncrementId => true;

        public async Task<TAggregateRoot> AddAsync(TAggregateRoot itemToCreate,
            CancellationToken cancellationToken = default)
        {
            if (AutoIncrementId)
            {
                // mock DB auto-increment id
                int newId = Interlocked.Increment(ref _identityCounter);
                Type t = itemToCreate.GetType();
                t.GetProperty("Id").SetValue(itemToCreate, newId);
            }

            await Task.Delay(100);
            MockSet.Add(itemToCreate);
            return itemToCreate;
        }

        public async Task<TAggregateRoot> FindByIdAsync(int itemId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100);
            return MockSet.FirstOrDefault(o => o.Id == itemId);
        }

        public async Task<TAggregateRoot> UpdateAsync(TAggregateRoot itemToUpdate, CancellationToken token = default)
        {
            await Task.Delay(100);
            var itemToReplace = MockSet.FirstOrDefault(o => o.Id == itemToUpdate.Id);
            if (itemToReplace != null) MockSet.Remove(itemToReplace);
            MockSet.Add(itemToUpdate);

            return itemToUpdate;
        }
    }
}