using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Commands;
using OzonEdu.StockApi.Grpc;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationService.Handlers
{
    public class ReserveOrderInStockCommandHandler : IRequestHandler<ReserveOrderInStockCommand>
    {
        private readonly ILogger<ReserveOrderInStockCommandHandler> _logger;
        private readonly ITracer _tracer;
        private readonly StockApiGrpc.StockApiGrpcClient _stockApiGrpcClient;
        private readonly IMerchOrderRepository _merchOrderRepository;

        public ReserveOrderInStockCommandHandler(IMerchOrderRepository merchOrderRepository,
            ILogger<ReserveOrderInStockCommandHandler> logger,
            ITracer tracer,
            StockApiGrpc.StockApiGrpcClient stockApiGrpcClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            _stockApiGrpcClient = stockApiGrpcClient ?? throw new ArgumentNullException(nameof(stockApiGrpcClient));
            _merchOrderRepository =
                merchOrderRepository ?? throw new ArgumentNullException(nameof(merchOrderRepository));
        }

        public async Task<Unit> Handle(ReserveOrderInStockCommand request, CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan(nameof(ReserveOrderInStockCommandHandler)).StartActive();
            
            var order = request.Order;
            var pendingItems = order.OrderItems.Where(item => item.Status.Equals(OrderItemStatus.Pending));

            foreach (var item in pendingItems)
            {
                try
                {
                    var grpcRequest = new GiveOutItemsRequest()
                    {
                        Items =
                        {
                            new SkuQuantityItem()
                            {
                                Quantity = item.Quantity.Value,
                                Sku = item.Sku.Value
                            }
                        }
                    };

                    var grpcResponse = await _stockApiGrpcClient.GiveOutItemsAsync(grpcRequest, cancellationToken: cancellationToken);

                    if (grpcResponse.Result == GiveOutItemsResponse.Types.Result.Successful)
                    {
                        item.SetStatusTo(OrderItemStatus.Reserved, DateTime.UtcNow);
                        _logger.LogInformation("Order {OrderId}. Stock-api reserved Sku {Sku}. Quantity: {Quantity}.",
                            order.Id.ToString(), item.Sku, item.Quantity.Value);
                    }
                    else
                    {
                        _logger.LogInformation("Order {OrderId}. Stock-api out of stock Sku {Sku}.",
                            order.Id.ToString(), item.Sku);
                    }
                }
                catch (RpcException e)
                {
                    _logger.LogError(e, "Failed grpc call to stock-api. Sku {Sku}. Order Id: {OrderId}.",
                        item.Sku, order.Id);
                }
            }

            // check if there are still Pending items
            var pendingItemsCount = order.OrderItems.Count(item => item.Status.Equals(OrderItemStatus.Pending));

            if (pendingItemsCount == 0) order.SetReservedStatus(DateTime.UtcNow);
            else order.SetDeferredStatus(DateTime.UtcNow);

            await _merchOrderRepository.UpdateAsync(order, cancellationToken);

            return Unit.Value;
        }
    }
}