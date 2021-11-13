using System.Collections.Generic;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Queries
{
    public class GetReservedOrdersByEmployeeIdQuery : IRequest<IEnumerable<MerchOrder>>
    {
        public GetReservedOrdersByEmployeeIdQuery(int employeeId)
        {
            EmployeeId = employeeId;
        }

        public int EmployeeId { get; }
    }
}