using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Domain.Events.MerchOrderAggregate
{
    public class OrderAssignedDomainEvent : INotification
    {
        public OrderAssignedDomainEvent(MerchOrder order)
        {
            Order = order;
        }

        public MerchOrder Order { get; }
    }
}