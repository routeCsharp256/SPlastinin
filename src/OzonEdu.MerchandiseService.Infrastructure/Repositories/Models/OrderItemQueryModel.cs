using System;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Models
{
    public class OrderItemQueryModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public long Sku { get; set; }
        public string SkuDescription { get; set; }
        public int Quantity { get; set; }
    }
}