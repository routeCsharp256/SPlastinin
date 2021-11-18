using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers
{
    public class
        GetReservedOrdersByEmployeeIdQueryHandler : IRequestHandler<GetReservedOrdersByEmployeeIdQuery,
            IEnumerable<MerchOrder>>
    {
        private readonly IMerchOrderRepository _merchOrderRepository;

        public GetReservedOrdersByEmployeeIdQueryHandler(IMerchOrderRepository merchOrderRepository)
        {
            _merchOrderRepository = merchOrderRepository;
        }

        public async Task<IEnumerable<MerchOrder>> Handle(GetReservedOrdersByEmployeeIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _merchOrderRepository.GetReservedByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        }
    }
}