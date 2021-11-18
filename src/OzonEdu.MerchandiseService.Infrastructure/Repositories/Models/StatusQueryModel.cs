using System;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Models
{
    public class StatusQueryModel
    {
        public int Id { get; set; }
        public DateTime StatusDate { get; set; }
        public string StatusDescription { get; set; }
    }
}