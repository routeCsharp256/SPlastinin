using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands
{
    public class ReserveOrderInStockCommand : IRequest
    {
        public ReserveOrderInStockCommand(MerchOrder order)
        {
            Order = order;
        }

        public MerchOrder Order { get; }
    }
}