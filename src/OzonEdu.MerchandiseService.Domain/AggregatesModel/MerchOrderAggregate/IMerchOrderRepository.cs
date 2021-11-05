using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate
{
    public interface IMerchOrderRepository : IRepository<MerchOrder>
    {
        Task<MerchOrder> FindByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<OrderItem>> GetCompletedByEmployeeIdAsync(int employeeId,
            CancellationToken cancellationToken = default);

        Task<int> GetIssuedToEmployeeQuantityLastYearBySkuAsync(int employeeId, long skuValue);

        Task<IEnumerable<MerchOrder>> GetReservedByEmployeeIdAsync(int employeeId,
            CancellationToken cancellationToken = default);
    }
}