using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.SeedWork;
using OzonEdu.MerchandiseService.HttpModels;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Validators;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers
{
    public class CreateMerchOrderCommandHandler :
        IRequestHandler<CreateMerchOrderCommand, CreateMerchOrderCommandResponse>
    {
        private readonly ILogger<CreateMerchOrderCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMerchOrderRepository _merchOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateMerchOrderCommandHandler(IMerchOrderRepository merchOrderRepository,
            IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork,
            ILogger<CreateMerchOrderCommandHandler> logger)
        {
            _merchOrderRepository =
                merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));

            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

                await _unitOfWork.StartTransaction(cancellationToken);
                
                var employee = await _employeeRepository.FindOrCreateEmployee(request.EmployeeId, request.EmployeeFirstName,
                    request.EmployeeLastName, request.EmployeeMiddleName, request.EmployeeEmail, cancellationToken);
                var manager = await _employeeRepository.FindOrCreateEmployee(request.ManagerId, request.ManagerFirstName,
                    request.ManagerLastName, request.ManagerMiddleName, request.ManagerEmail, cancellationToken);

                var newOrder = new MerchOrder(employee);

                foreach (var orderItem in request.OrderItems)
                {
                    newOrder.AddOrderItem(orderItem.Sku, orderItem.Description, orderItem.Quantity, DateTime.UtcNow);
                }

                newOrder.AssignTo(manager, DateTime.UtcNow);

                await CancelAlreadyIssuedOrderItems(newOrder);

                newOrder = await _merchOrderRepository.AddAsync(newOrder, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

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
    }
}