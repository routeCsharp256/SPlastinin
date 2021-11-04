using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Events
{
    public class NotifyWhenOrderReservedEvent : INotification
    {
        public NotifyWhenOrderReservedEvent(MerchOrder order)
        {
            Order = order;
        }

        public MerchOrder Order { get; }
    }
}