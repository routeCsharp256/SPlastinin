using System.Collections.Generic;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries
{
    public class GetCompletedOrdersByEmployeeEmailQuery : IRequest<IEnumerable<OrderItem>>
    {
        public GetCompletedOrdersByEmployeeEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}