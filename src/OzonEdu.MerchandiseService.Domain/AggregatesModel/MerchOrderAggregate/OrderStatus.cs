using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate
{
    public sealed class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Draft = new(1, nameof(Draft), nameof(Draft));
        public static readonly OrderStatus Created = new(2, nameof(Created), nameof(Created));
        public static readonly OrderStatus Assigned = new(3, nameof(Assigned), "Assigned to manager");
        public static readonly OrderStatus Canceled = new(4, nameof(Canceled), nameof(Canceled));

        public static readonly OrderStatus InProgress = new(5, nameof(InProgress),
            "Waiting for confirmation of stock reserve.");

        public static readonly OrderStatus Reserved = new(6, nameof(Reserved),
            "The order has been reserved in the stock.");

        public static readonly OrderStatus Completed = new(7, nameof(Completed),
            "The order has been issued to the employee.");

        public static readonly OrderStatus Deferred = new(8, nameof(Deferred),
            "The order has been deferred until the supply of the merch.");

        public string DefaultDescription { get; }

        public OrderStatus(int id, string name, string defaultDescription) : base(id, name)
        {
            DefaultDescription = defaultDescription;
        }
    }
}