using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate
{
    public sealed class OrderItemStatus : Enumeration
    {
        public static readonly OrderItemStatus Pending = new(1, nameof(Pending), nameof(Pending));
        public static readonly OrderItemStatus Canceled = new(2, nameof(Canceled), nameof(Canceled));
        public static readonly OrderItemStatus Reserved = new(3, nameof(Reserved),
            "The merch has been reserved in the stock.");
        public static readonly OrderItemStatus Issued = new(4, nameof(Issued),
            "The merch has been issued to the employee.");

        public string DefaultDescription { get; }

        public OrderItemStatus(int id, string name, string defaultDescription) : base(id, name)
        {
            DefaultDescription = defaultDescription;
        }
    }
}