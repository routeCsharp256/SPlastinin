using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Domain.Events.MerchOrderAggregate
{
    public class OrderReassignedDomainEvent : INotification
    {
        public OrderReassignedDomainEvent(MerchOrder order)
        {
            Order = order;
        }

        public MerchOrder Order { get; }
    }
}