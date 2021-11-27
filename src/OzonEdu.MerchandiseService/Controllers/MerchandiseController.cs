using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzonEdu.MerchandiseService.HttpModels;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries;

namespace OzonEdu.MerchandiseService.Controllers
{
    [ApiController]
    [Route("v1/api/merch")]
    [Produces("application/json")]
    public class MerchandiseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MerchandiseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a merch order
        /// </summary>
        /// <param name="model">Contains order data: employeeId, skuId, quantity, etc</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>MerchOrderResponse</returns>
        [HttpPost]
        public async Task<ActionResult<MerchOrderResponse>> CreateMerchOrder([FromBody] MerchOrderPostViewModel model,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateMerchOrderCommand()
            {
                EmployeeId = model.EmployeeId,
                EmployeeFirstName = model.EmployeeFirstName,
                EmployeeLastName = model.EmployeeLastName,
                EmployeeMiddleName = model.EmployeeMiddleName,
                EmployeeEmail = model.EmployeeEmail,
                OrderItems = model.OrderItems,
                ManagerId = model.ManagerId,
                ManagerFirstName = model.ManagerFirstName,
                ManagerLastName = model.ManagerLastName,
                ManagerMiddleName = model.ManagerMiddleName,
                ManagerEmail = model.ManagerEmail
            };

            var commandResponse = await _mediator.Send(command, cancellationToken);

            var response = new MerchOrderResponse()
            {
                OrderId = commandResponse.Id,
                Status = commandResponse.Status,
                Description = commandResponse.Description
            };

            return Ok(response);
        }

        /// <summary>
        /// Returns a list of merch passed to an employee.
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<MerchItemDto>>> GetMerchListByEmployeeId(int employeeId,
            CancellationToken cancellationToken)
        {
            var query = new GetCompletedOrdersByEmployeeIdQuery(employeeId);
            var skuList = await _mediator.Send(query, cancellationToken);

            var response = new List<MerchItemDto>();
            foreach (var item in skuList)
            {
                response.Add(new MerchItemDto()
                {
                    Sku = item.Sku.Value,
                    Description = item.Sku.Description,
                    Quantity = item.Quantity.Value,
                    IssueDate = item.StatusDate
                });
            }

            return response;
        }

        /// <summary>
        /// Returns a list of merch orders reserved for an employee.
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("employee/{employeeId:int}/reserved")]
        public async Task<ActionResult<IEnumerable<ReservedOrderResponse>>> GetReservedMerchOrdersByEmployeeId(
            int employeeId,
            CancellationToken cancellationToken)
        {
            var query = new GetReservedOrdersByEmployeeIdQuery(employeeId);
            var orderList = await _mediator.Send(query, cancellationToken);

            var response = new List<ReservedOrderResponse>();
            foreach (var item in orderList)
            {
                var orderItems = new List<MerchItemDto>();
                foreach (var orderItem in item.OrderItems)
                {
                    orderItems.Add(new MerchItemDto()
                    {
                        Sku = orderItem.Sku.Value,
                        Description = orderItem.Sku.Description,
                        Quantity = orderItem.Quantity.Value
                    });
                }

                response.Add(new ReservedOrderResponse()
                {
                    Id = item.Id,
                    OrderItems = orderItems
                });
            }

            return response;
        }
    }
}