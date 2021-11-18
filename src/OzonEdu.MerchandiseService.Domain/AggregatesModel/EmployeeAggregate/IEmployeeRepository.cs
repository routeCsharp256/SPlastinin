using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee> FindByIdAsync(int employeeId, CancellationToken cancellationToken);

        Task<Employee> FindOrCreateEmployee(int employeeId, string firstName, string lastName, string middleName,
            string email, CancellationToken cancellationToken);
    }
}