using System;
using System.ComponentModel.DataAnnotations;

namespace OzonEdu.MerchandiseService.HttpModels
{
    public class MerchItemDto
    {
        [Required] public long Sku { get; init; }
        public string Description { get; init; }
        [Required] public int Quantity { get; init; }
        public DateTime? IssueDate { get; init; }
    }
}