using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OzonEdu.MerchandiseService.HttpModels;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries;
using ILogger = Serilog.ILogger;

namespace OzonEdu.MerchandiseService.Controllers
{
    [ApiController]
    [Route("v1/api/merch")]
    [Produces("application/json")]
    public class MerchandiseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITracer _tracer;
        private readonly ILogger<MerchandiseController> _logger;

        public MerchandiseController(IMediator mediator, ITracer tracer, ILogger<MerchandiseController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a merch order
        /// </summary>
        /// <param name="model">Contains order data: employeeId, skuId, quantity, etc</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>MerchOrderResponse</returns>
        [HttpPost]
        public async Task<ActionResult<MerchOrderResponse>> CreateMerchOrder(MerchOrderPostViewModel model,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchandiseController)}.{nameof(CreateMerchOrder)}")
                .StartActive();

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
            using var span = _tracer.BuildSpan($"{nameof(MerchandiseController)}.{nameof(GetMerchListByEmployeeId)}")
                .StartActive();

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
        /// Returns a list of merch passed to an employee.
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet("employee/{email}")]
        public async Task<ActionResult<IEnumerable<MerchItemDto>>> GetMerchListByEmployeeEmail(string email,
            CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchandiseController)}.{nameof(GetMerchListByEmployeeEmail)}")
                .StartActive();

            var query = new GetCompletedOrdersByEmployeeEmailQuery(email);
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
            using var span = _tracer
                .BuildSpan($"{nameof(MerchandiseController)}.{nameof(GetReservedMerchOrdersByEmployeeId)}")
                .StartActive();

            var query = new GetReservedOrdersByEmployeeIdQuery(employeeId);
            var orderList = await _mediator.Send(query, cancellationToken);
            
            _logger.LogInformation("GetReservedMerchOrdersByEmployeeId {EmployeeId}. Order list: {@OrderList}",
                employeeId.ToString(), orderList);

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