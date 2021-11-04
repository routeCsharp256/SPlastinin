using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Queries;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers
{
    public class GetCompletedOrdersByEmployeeIdCommandHandler :
        IRequestHandler<GetCompletedOrdersByEmployeeIdQuery, IEnumerable<OrderItemWithIssueDate>>
    {
        private readonly IMerchOrderRepository _merchOrderRepository;

        public GetCompletedOrdersByEmployeeIdCommandHandler(IMerchOrderRepository merchOrderRepository)
        {
            _merchOrderRepository = merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
        }

        public async Task<IEnumerable<OrderItemWithIssueDate>> Handle(GetCompletedOrdersByEmployeeIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _merchOrderRepository.GetCompletedByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        }
    }
}