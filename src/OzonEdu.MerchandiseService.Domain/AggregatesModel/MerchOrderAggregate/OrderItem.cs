using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate
{
    public sealed class OrderItem : Entity
    {
        public OrderItem(
            int id,
            Sku sku,
            Quantity quantity,
            OrderItemStatus status,
            DateTime statusDate,
            string statusDescription)
        {
            Id = id;
            Sku = sku;
            Quantity = quantity;
            Status = status;
            StatusDate = statusDate;
            StatusDescription = statusDescription;
        }

        public Sku Sku { get; }
        public Quantity Quantity { get; private set; }
        public OrderItemStatus Status { get; private set; }
        public DateTime StatusDate { get; private set; }
        public string StatusDescription { get; private set; }

        private OrderItem(Sku sku, Quantity quantity, DateTime utcNow)
        {
            Sku = sku;
            Quantity = quantity;
            Status = OrderItemStatus.Pending;
            StatusDate = utcNow;
            StatusDescription = Status.DefaultDescription;
        }
        
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