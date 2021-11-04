using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate
{
    public interface IMerchOrderRepository : IRepository<MerchOrder>
    {
        Task<MerchOrder> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderItemWithIssueDate>> GetCompletedByEmployeeIdAsync(int employeeId,
            CancellationToken cancellationToken = default);
    }
}