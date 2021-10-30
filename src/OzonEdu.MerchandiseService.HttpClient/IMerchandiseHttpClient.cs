using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.HttpModels;

namespace OzonEdu.MerchandiseService.HttpClient
{
    public interface IMerchandiseHttpClient
    {
        Task<MerchOrderResponse> CreateMerchOrder(MerchOrderPostViewModel model, CancellationToken token);
        Task<List<MerchItemResponse>> GetMerchListByEmployeeId(int employeeId, CancellationToken token);
    }
}