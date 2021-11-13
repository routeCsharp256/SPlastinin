using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.HttpModels;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Validators;

namespace OzonEdu.MerchandiseService.Infrastructure.DomainService.Handlers
{
    public class CreateMerchOrderCommandHandler :
        IRequestHandler<CreateMerchOrderCommand, CreateMerchOrderCommandResponse>
    {
        private readonly ILogger<CreateMerchOrderCommandHandler> _logger;
        private readonly IMerchOrderRepository _merchOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateMerchOrderCommandHandler(IMerchOrderRepository merchOrderRepository,
            IEmployeeRepository employeeRepository, ILogger<CreateMerchOrderCommandHandler> logger)
        {
            _merchOrderRepository =
                merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _logger = logger;
        }

        public async Task<CreateMerchOrderCommandResponse> Handle(CreateMerchOrderCommand request,
            CancellationToken cancellationToken)
        {
            CreateMerchOrderCommandResponse response = null;

            try
            {
                var validateResult = CreateMerchOrderCommandValidator.ValidateCommand(request);
                if (validateResult.Count > 0)
                {
                    return new CreateMerchOrderCommandResponse()
                    {
                        Id = 0,
                        Status = "Error",
                        Description = JsonSerializer.Serialize(validateResult)
                    };
                }

                var employee = await FindOrCreateEmployee(request.EmployeeId, request.EmployeeFirstName,
                    request.EmployeeLastName, request.EmployeeMiddleName, request.EmployeeEmail, cancellationToken);
                var manager = await FindOrCreateEmployee(request.ManagerId, request.ManagerFirstName,
                    request.ManagerLastName, request.ManagerMiddleName, request.ManagerEmail, cancellationToken);

                var newOrder = new MerchOrder(employee);

                foreach (var orderItem in request.OrderItems)
                {
                    newOrder.AddOrderItem(orderItem.Sku, orderItem.Description, orderItem.Quantity, DateTime.UtcNow);
                }

                newOrder.AssignTo(manager, DateTime.UtcNow);

                await CancelAlreadyIssuedOrderItems(newOrder);

                newOrder = await _merchOrderRepository.AddAsync(newOrder, cancellationToken);
                await _merchOrderRepository.SaveEntitiesAsync(cancellationToken);

                response = new CreateMerchOrderCommandResponse()
                {
                    Id = newOrder.Id,
                    Status = "Success",
                    Description = "Order created"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());

                response = new CreateMerchOrderCommandResponse()
                {
                    Id = 0,
                    Status = "Error",
                    Description = ""
                };
            }

            return response;
        }

        private async Task CancelAlreadyIssuedOrderItems(MerchOrder order)
        {
            try
            {
                List<MerchItemDto> addItemList = new();
                foreach (var item in order.OrderItems)
                {
                    int alreadyIssuedQuantity =
                        await _merchOrderRepository.GetIssuedToEmployeeQuantityLastYearBySkuAsync(order.Receiver.Id,
                            item.Sku.Value);
                    if (alreadyIssuedQuantity >= item.Quantity.Value)
                    {
                        item.SetStatusTo(OrderItemStatus.Canceled, DateTime.UtcNow,
                            $"Merch item has already been issued to employee less than year ago. Issued Quantity: {alreadyIssuedQuantity}");
                    }
                    else if (alreadyIssuedQuantity > 0)
                    {
                        var newItem = new MerchItemDto()
                        {
                            Sku = item.Sku.Value,
                            Description = item.Sku.Description,
                            Quantity = item.Quantity.Value - alreadyIssuedQuantity
                        };

                        addItemList.Add(newItem);

                        item.SetQuantityTo(alreadyIssuedQuantity);
                        item.SetStatusTo(OrderItemStatus.Canceled, DateTime.UtcNow,
                            $"Merch item has already been issued to employee less than year ago. Issued Quantity: {alreadyIssuedQuantity}");
                    }
                }

                foreach (var item in addItemList)
                {
                    order.AddOrderItem(item.Sku, item.Description, item.Quantity, DateTime.UtcNow);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        private async Task<Employee> FindOrCreateEmployee(int id, string firstName, string lastName, string middleName,
            string email, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.FindByIdAsync(id, cancellationToken);
            bool employeeOriginallyExisted = (employee != null);

            if (!employeeOriginallyExisted)
            {
                employee = Employee.Create(id, firstName, lastName, middleName, email);
            }

            var savedEmployee = employeeOriginallyExisted
                ? await _employeeRepository.UpdateAsync(employee, cancellationToken)
                : await _employeeRepository.AddAsync(employee, cancellationToken);

            await _employeeRepository.SaveEntitiesAsync(cancellationToken);

            return savedEmployee;
        }
    }
}