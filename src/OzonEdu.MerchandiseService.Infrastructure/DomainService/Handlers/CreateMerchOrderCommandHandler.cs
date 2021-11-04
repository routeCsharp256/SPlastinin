using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;

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
                var validateResult = ValidateRequest(request);
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

                var newOrder = MerchOrder.Create(employee,
                    OrderItem.Create(request.Sku, request.SkuDescription, request.Quantity),
                    manager, DateTime.UtcNow);

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

        private Dictionary<string, string> ValidateRequest(CreateMerchOrderCommand request)
        {
            Dictionary<string, string> result = new();

            if (!Email.IsEmailValid(request.EmployeeEmail))
            {
                result[nameof(request.EmployeeEmail)] = "Employee's email is not valid";
            }

            if (!Email.IsEmailValid(request.ManagerEmail))
            {
                result[nameof(request.ManagerEmail)] = "Manager's email is not valid";
            }

            if (request.Quantity <= 0)
            {
                result[nameof(request.Quantity)] = "Quantity must be greater than zero";
            }

            return result;
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