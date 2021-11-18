using System.Collections.Generic;
using MediatR;
using OzonEdu.MerchandiseService.HttpModels;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands
{
    public class CreateMerchOrderCommand : IRequest<CreateMerchOrderCommandResponse>
    {
        public int EmployeeId { get; init; }
        public string EmployeeLastName { get; init; }
        public string EmployeeFirstName { get; init; }
        public string EmployeeMiddleName { get; init; }
        public string EmployeeEmail { get; init; }
        public IEnumerable<MerchItemDto> OrderItems { get; init; }
        public int ManagerId { get; init; }
        public string ManagerLastName { get; init; }
        public string ManagerFirstName { get; init; }
        public string ManagerMiddleName { get; init; }
        public string ManagerEmail { get; init; }
    }
}