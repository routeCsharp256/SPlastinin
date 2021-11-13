using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Validators
{
    public class CreateMerchOrderCommandValidator
    {
        public static Dictionary<string, string> ValidateCommand(CreateMerchOrderCommand command)
        {
            Dictionary<string, string> result = new();

            if (!Email.IsEmailValid(command.EmployeeEmail))
            {
                result[nameof(command.EmployeeEmail)] = "Employee's email is not valid";
            }

            if (!Email.IsEmailValid(command.ManagerEmail))
            {
                result[nameof(command.ManagerEmail)] = "Manager's email is not valid";
            }

            foreach (var item in command.OrderItems)
            {
                if (item.Quantity <= 0)
                {
                    result[nameof(item.Quantity)] =
                        $"Order item {item.Description} (SKU {item.Sku}) - Quantity must be greater than zero";
                }
            }

            return result;
        }
    }
}