using System;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Models
{
    public class MerchOrderQueryModel
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusDescription { get; set; }
    }
}