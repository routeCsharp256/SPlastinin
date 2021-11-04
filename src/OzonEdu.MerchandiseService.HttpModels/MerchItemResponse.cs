using System;

namespace OzonEdu.MerchandiseService.HttpModels
{
    public class MerchItemResponse
    {
        public long Sku { get; init; }
        public string Description { get; init; }
        public int Quantity { get; init; }
        public DateTime IssueDate { get; init; }
    }
}