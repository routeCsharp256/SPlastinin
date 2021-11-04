using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands
{
    public class CreateMerchOrderCommand : IRequest<CreateMerchOrderCommandResponse>
    {
        public int EmployeeId { get; init; }
        public string EmployeeLastName { get; init; }
        public string EmployeeFirstName { get; init; }
        public string EmployeeMiddleName { get; init; }
        public string EmployeeEmail { get; init; }
        public long Sku { get; init; }
        public string SkuDescription { get; init; }
        public int Quantity { get; init; }
        public int ManagerId { get; init; }
        public string ManagerLastName { get; init; }
        public string ManagerFirstName { get; init; }
        public string ManagerMiddleName { get; init; }
        public string ManagerEmail { get; init; }
    }
}