using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using OzonEdu.MerchandiseService.Grpc;
using Google.Protobuf.WellKnownTypes;
using OpenTracing;
using OzonEdu.MerchandiseService.HttpModels;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Queries;
using MerchOrderResponse = OzonEdu.MerchandiseService.Grpc.MerchOrderResponse;

namespace OzonEdu.MerchandiseService.GrpcServices
{
    public class MerchandiseGrpcService : MerchandiseGrpc.MerchandiseGrpcBase
    {
        private readonly IMediator _mediator;
        private readonly ITracer _tracer;

        public MerchandiseGrpcService(IMediator mediator, ITracer tracer)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
        }

        public override async Task<GetMerchOrderListResponse> GetReservedMerchOrdersByEmployeeId(GetMerchListRequest request, ServerCallContext context)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchandiseGrpcService)}.{nameof(GetReservedMerchOrdersByEmployeeId)}").StartActive();
            
            var query = new GetReservedOrdersByEmployeeIdQuery(request.EmployeeId);
            var orderList = await _mediator.Send(query, context.CancellationToken);

            var response = new GetMerchOrderListResponse();
            foreach (var item in orderList)
            {
                var orderItems = new List<MerchItemResponse>();
                foreach (var orderItem in item.OrderItems)
                {
                    orderItems.Add(new MerchItemResponse()
                    {
                        Sku = orderItem.Sku.Value,
                        Description = orderItem.Sku.Description,
                        Quantity = orderItem.Quantity.Value
                    });
                }

                var order = new MerchOrderResponse();
                order.OrderId = item.Id;
                order.MerchList.AddRange(orderItems);
                response.OrderList.Add(order);
            }

            return response;
        }

        public override async Task<GetMerchListResponse> GetMerchListByEmployeeId(GetMerchListRequest request,
            ServerCallContext context)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchandiseGrpcService)}.{nameof(GetMerchListByEmployeeId)}").StartActive();
            
            var query = new GetCompletedOrdersByEmployeeIdQuery(request.EmployeeId);
            var skuList = await _mediator.Send(query, context.CancellationToken);

            var response = new GetMerchListResponse();
            foreach (var item in skuList)
            {
                response.MerchList.Add(new MerchItemResponse()
                {
                    Sku = item.Sku.Value,
                    Description = item.Sku.Description,
                    Quantity = item.Quantity.Value,
                    IssueDate = Timestamp.FromDateTime(DateTime.SpecifyKind(item.StatusDate, DateTimeKind.Utc))
                });
            }

            return response;
        }

        public override async Task<CreateMerchOrderResponse> CreateMerchOrder(CreateMerchOrderRequest request,
            ServerCallContext context)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchandiseGrpcService)}.{nameof(CreateMerchOrder)}").StartActive();
            
            var orderItems = new List<MerchItemDto>();
            foreach (var item in request.OrderItems)
            {
                orderItems.Add(new MerchItemDto()
                {
                    Sku = item.Sku,
                    Quantity = item.Quantity,
                    Description = item.SkuDescription
                });
            }

            var command = new CreateMerchOrderCommand()
            {
                EmployeeId = request.EmployeeId,
                EmployeeFirstName = request.EmployeeFirstName,
                EmployeeLastName = request.EmployeeLastName,
                EmployeeMiddleName = request.EmployeeMiddleName,
                EmployeeEmail = request.EmployeeEmail,
                OrderItems = orderItems,
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