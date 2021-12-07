using System.Collections.Generic;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries
{
    public class GetReservedOrdersByEmployeeEmailQuery : IRequest<IEnumerable<MerchOrder>>
    {
        public GetReservedOrdersByEmployeeEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}