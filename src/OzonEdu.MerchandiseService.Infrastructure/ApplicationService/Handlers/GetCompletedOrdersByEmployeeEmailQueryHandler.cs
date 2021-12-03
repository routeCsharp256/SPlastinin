using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OpenTracing;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers
{
    public class GetCompletedOrdersByEmployeeEmailQueryHandler :
        IRequestHandler<GetCompletedOrdersByEmployeeEmailQuery, IEnumerable<OrderItem>>
    {
        private readonly IMerchOrderRepository _merchOrderRepository;
        private readonly ITracer _tracer;

        public GetCompletedOrdersByEmployeeEmailQueryHandler(IMerchOrderRepository merchOrderRepository,
            ITracer tracer)
        {
            _merchOrderRepository = merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public async Task<IEnumerable<OrderItem>> Handle(GetCompletedOrdersByEmployeeEmailQuery request,
            CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan(nameof(GetCompletedOrdersByEmployeeEmailQueryHandler)).StartActive();
            
            return await _merchOrderRepository.GetCompletedByEmployeeEmailAsync(request.Email, cancellationToken);
        }
    }
}