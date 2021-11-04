using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.HttpModels;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Queries;

namespace OzonEdu.MerchandiseService.Controllers
{
    [ApiController]
    [Route("v1/api/merch")]
    [Produces("application/json")]
    public partial class MerchandiseController : ControllerBase
    {
        private readonly IMerchOrderRepository _repository;
        private readonly IMediator _mediator;

        public MerchandiseController(IMerchOrderRepository repository, IMediator mediator)
        {
            _repository = repository;
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
                Sku = model.Sku,
                SkuDescription = model.SkuDescription,
                Quantity = model.Quantity,
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
        public async Task<ActionResult<IEnumerable<MerchItemResponse>>> GetMerchListByEmployeeId(int employeeId,
            CancellationToken cancellationToken)
        {
            var command = new GetCompletedOrdersByEmployeeIdQuery(employeeId);
            var skuList = await _mediator.Send(command, cancellationToken);

            var response = new List<MerchItemResponse>();
            foreach (var item in skuList)
            {
                response.Add(new MerchItemResponse()
                { 
                    Sku = item.Item.Sku.Value,
                    Description = item.Item.Sku.Description,
                    Quantity = item.Item.Quantity.Value,
                    IssueDate = item.IssueDate
                });
            }
            
            return response;
        }
    }
}