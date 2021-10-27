using System.Threading.Tasks;
using Grpc.Core;
using OzonEdu.MerchandiseService.Grpc;

namespace OzonEdu.MerchandiseService.GrpcServices
{
    public class MerchandiseGrpcService: MerchandiseGrpc.MerchandiseGrpcBase
    {
        public override Task<GetMerchListResponse> GetMerchListByEmployeeId(GetMerchListRequest request, ServerCallContext context)
        {
            return base.GetMerchListByEmployeeId(request, context);
        }

        public override Task<CreateMerchOrderResponse> CreateMerchOrder(CreateMerchOrderRequest request, ServerCallContext context)
        {
            return base.CreateMerchOrder(request, context);
        }
    }
}