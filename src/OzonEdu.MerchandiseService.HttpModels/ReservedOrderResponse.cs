using System;
using System.Collections.Generic;

namespace OzonEdu.MerchandiseService.HttpModels
{
    public class ReservedOrderResponse
    {
        public int Id { get; set; }
        public List<MerchItemDto> OrderItems { get; set; }
    }
}