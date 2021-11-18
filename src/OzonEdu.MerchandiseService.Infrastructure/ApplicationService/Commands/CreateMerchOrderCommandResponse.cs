namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands
{
    public class CreateMerchOrderCommandResponse
    {
        public int Id { get; init; }
        public string Status { get; init; }
        public string Description { get; init; }
    }
}