using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Events
{
    public class NotifyWhenOrderDeferredEvent : INotification
    {
        public NotifyWhenOrderDeferredEvent(MerchOrder order)
        {
            Order = order;
        }

        public MerchOrder Order { get; }
    }
}