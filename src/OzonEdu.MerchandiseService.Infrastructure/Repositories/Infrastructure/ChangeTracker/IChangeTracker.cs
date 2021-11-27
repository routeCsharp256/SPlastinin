using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.ChangeTracker
{
    public interface IChangeTracker
    {
        IEnumerable<Entity> TrackedEntities { get; }

        public void Track(Entity entity);
    }
}