using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Mocks
{
    public class MockEmployeeRepository : MockRepository<Employee>, IEmployeeRepository
    {
        public MockEmployeeRepository()
        {
        }

        protected override bool AutoIncrementId => false;
        public Task SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}