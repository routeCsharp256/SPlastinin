using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Events
{
    public class NotifyWhenOrderCanceledEvent : INotification
    {
        public NotifyWhenOrderCanceledEvent(MerchOrder order)
        {
            Order = order;
        }

        public MerchOrder Order { get; }
    }
}