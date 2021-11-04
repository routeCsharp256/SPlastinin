using System.Collections.Generic;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Queries
{
    public class GetCompletedOrdersByEmployeeIdQuery : IRequest<IEnumerable<OrderItemWithIssueDate>>
    {
        public GetCompletedOrdersByEmployeeIdQuery(int employeeId)
        {
            EmployeeId = employeeId;
        }

        public int EmployeeId { get; }
    }
}