using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate
{
    public sealed class OrderItem : Entity
    {
        private OrderItem(Sku sku, Quantity quantity, DateTime utcNow)
        {
            Sku = sku;
            Quantity = quantity;
            Status = OrderItemStatus.Pending;
            StatusDate = utcNow;
        }

        public Sku Sku { get; }
        public Quantity Quantity { get; private set; }
        public OrderItemStatus Status { get; private set; }
        public DateTime StatusDate { get; private set; }
        public string StatusDescription { get; private set; }

        public static OrderItem Create(long sku, string skuDescription, int quantity, DateTime utcNow)
        {
            return new OrderItem(
                new Sku(sku, skuDescription),
                new Quantity(quantity),
                utcNow);
        }

        public void SetStatusTo(OrderItemStatus newStatus, DateTime utcNow, string message = null)
        {
            Status = newStatus;
            StatusDate = utcNow;
            StatusDescription = message ?? Status.DefaultDescription;
        }

        public void SetQuantityTo(int newQuantityValue)
        {
            Quantity = new Quantity(newQuantityValue);
        }

        public override string ToString()
        {
            return $"{Sku} (Quantity: {Quantity.Value})";
        }
    }
}