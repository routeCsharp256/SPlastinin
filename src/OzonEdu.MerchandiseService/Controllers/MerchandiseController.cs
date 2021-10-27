using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OzonEdu.MerchandiseService.HttpModels;

namespace OzonEdu.MerchandiseService.Controllers
{
    [ApiController]
    [Route("v1/api/merch")]
    [Produces("application/json")]
    public partial class MerchandiseController : ControllerBase
    {
        /// <summary>
        /// Creates a merch order
        /// </summary>
        /// <param name="model">Contains order data: employeeId, skuId, quantity, etc</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>MerchOrderResponse</returns>
        [HttpPost]
        public async Task<ActionResult<MerchOrderResponse>> CreateMerchOrder(MerchOrderPostViewModel model, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of merch passed to an employee.
        /// </summary>
        /// <param name="employeeId">Employee Id</param>
        /// <param name="token">Cancellation token</param>
        [HttpGet("{employeeId:int}")]
        public async Task<ActionResult<List<MerchItemResponse>>> GetMerchListByEmployeeId(int employeeId, CancellationToken token)
        {
            throw new NotImplementedException();
        }
        
    }
}