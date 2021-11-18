using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Validators
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
                if (!Quantity.IsValueValid(item.Quantity))
                {
                    result[nameof(item.Quantity)] =
                        $"Order item {item.Description} (SKU {item.Sku}) - Quantity is not valid";
                }
            }

            return result;
        }
    }
}