using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects
{
    public sealed class OrderItem : ValueObject
    {
        private OrderItem(Sku sku, Quantity quantity)
        {
            Sku = sku;
            Quantity = quantity;
        }

        public Sku Sku { get; }
        public Quantity Quantity { get; }

        public static OrderItem Create(long sku, string skuDescription, int quantity)
        {
            return new OrderItem(
                new Sku(sku, skuDescription),
                new Quantity(quantity));
        }

        public override string ToString()
        {
            return $"{Sku} (Quantity: {Quantity.Value})";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Sku;
            yield return Quantity;
        }
    }
}