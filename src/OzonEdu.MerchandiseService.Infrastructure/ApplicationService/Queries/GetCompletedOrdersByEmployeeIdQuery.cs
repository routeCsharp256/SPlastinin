using System.Collections.Generic;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries
{
    public class GetCompletedOrdersByEmployeeIdQuery : IRequest<IEnumerable<OrderItem>>
    {
        public GetCompletedOrdersByEmployeeIdQuery(int employeeId)
        {
            EmployeeId = employeeId;
        }

        public int EmployeeId { get; }
    }
}