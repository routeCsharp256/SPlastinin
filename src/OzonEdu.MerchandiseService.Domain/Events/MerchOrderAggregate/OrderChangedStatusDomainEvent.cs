using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Domain.Events.MerchOrderAggregate
{
    public class OrderChangedStatusDomainEvent : INotification
    {
        public MerchOrder Order { get; }

        public OrderChangedStatusDomainEvent(MerchOrder order)
        {
            Order = order;
        }
    }
}