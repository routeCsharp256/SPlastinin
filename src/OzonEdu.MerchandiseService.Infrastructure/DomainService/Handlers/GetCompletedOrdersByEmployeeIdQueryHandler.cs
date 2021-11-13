using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Queries;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers
{
    public class GetCompletedOrdersByEmployeeIdQueryHandler :
        IRequestHandler<GetCompletedOrdersByEmployeeIdQuery, IEnumerable<OrderItem>>
    {
        private readonly IMerchOrderRepository _merchOrderRepository;

        public GetCompletedOrdersByEmployeeIdQueryHandler(IMerchOrderRepository merchOrderRepository)
        {
            _merchOrderRepository = merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
        }

        public async Task<IEnumerable<OrderItem>> Handle(GetCompletedOrdersByEmployeeIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _merchOrderRepository.GetCompletedByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        }
    }
}