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
    public class GetReservedOrdersByEmployeeEmailQueryHandler : IRequestHandler<GetReservedOrdersByEmployeeEmailQuery,
            IEnumerable<MerchOrder>>
    {
        private readonly IMerchOrderRepository _merchOrderRepository;
        private readonly ITracer _tracer;

        public GetReservedOrdersByEmployeeEmailQueryHandler(IMerchOrderRepository merchOrderRepository, ITracer tracer)
        {
            _merchOrderRepository = merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public async Task<IEnumerable<MerchOrder>> Handle(GetReservedOrdersByEmployeeEmailQuery request,
            CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan(nameof(GetReservedOrdersByEmployeeEmailQueryHandler)).StartActive();
            
            return await _merchOrderRepository.GetReservedByEmployeeEmailAsync(request.Email, cancellationToken);
        }
    }
}