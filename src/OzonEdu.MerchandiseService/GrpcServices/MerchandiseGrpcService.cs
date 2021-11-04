using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using OzonEdu.MerchandiseService.Grpc;
using Google.Protobuf.WellKnownTypes;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.DomainService.Queries;

namespace OzonEdu.MerchandiseService.GrpcServices
{
    public class MerchandiseGrpcService : MerchandiseGrpc.MerchandiseGrpcBase
    {
        private readonly IMediator _mediator;

        public MerchandiseGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetMerchListResponse> GetMerchListByEmployeeId(GetMerchListRequest request,
            ServerCallContext context)
        {
            var command = new GetCompletedOrdersByEmployeeIdQuery(request.EmployeeId);
            var skuList = await _mediator.Send(command, context.CancellationToken);

            var response = new GetMerchListResponse();
            foreach (var item in skuList)
            {
                response.MerchList.Add(new IssuedMerchItem()
                {
                    Sku = item.Item.Sku.Value,
                    Description = item.Item.Sku.Description,
                    Quantity = item.Item.Quantity.Value,
                    IssueDate = Timestamp.FromDateTime(item.IssueDate)
                });
            }

            return response;
        }

        public override async Task<CreateMerchOrderResponse> CreateMerchOrder(CreateMerchOrderRequest request,
            ServerCallContext context)
        {
            var command = new CreateMerchOrderCommand()
            {
                EmployeeId = request.EmployeeId,
                EmployeeFirstName = request.EmployeeFirstName,
                EmployeeLastName = request.EmployeeLastName,
                EmployeeMiddleName = request.EmployeeMiddleName,
                EmployeeEmail = request.EmployeeEmail,
                Sku = request.Sku,
                SkuDescription = request.SkuDescription,
                Quantity = request.Quantity,
                ManagerId = request.ManagerId,
                ManagerFirstName = request.ManagerFirstName,
                ManagerLastName = request.ManagerLastName,
                ManagerMiddleName = request.ManagerMiddleName,
                ManagerEmail = request.ManagerEmail
            };

            var order = await _mediator.Send(command, context.CancellationToken);

            var response = new CreateMerchOrderResponse()
            {
                OrderId = order.Id,
                Status = order.Status,
                Description = order.Description
            };

            return response;
        }
    }
}