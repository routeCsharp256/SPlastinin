using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTracing;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Domain.SeedWork;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.ChangeTracker;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.DbConnection;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Models;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Implementation
{
    public class MerchOrderRepository : IMerchOrderRepository
    {
        private readonly IDbConnectionFactory<NpgsqlConnection> _dbConnectionFactory;
        private readonly IChangeTracker _changeTracker;
        private readonly ITracer _tracer;
        private readonly ILogger<MerchOrderRepository> _logger;

        public MerchOrderRepository(IDbConnectionFactory<NpgsqlConnection> dbConnectionFactory,
            IChangeTracker changeTracker, ITracer tracer, ILogger<MerchOrderRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private const int Timeout = 5;

        public async Task<MerchOrder> AddAsync(MerchOrder itemToAdd, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(AddAsync)}").StartActive();
            
            if (!itemToAdd.IsTransient())
            {
                _changeTracker.Track(itemToAdd);
                return itemToAdd;
            }

            const string sql = @"
                INSERT INTO merch_orders (receiver_id, manager_id, status_id, status_date, status_description)
                VALUES (@ReceiverId, @ManagerId, @StatusId, @StatusDate, @StatusDescription)
                RETURNING id;";

            var parameters = new
            {
                ReceiverId = itemToAdd.Receiver.Id,
                ManagerId = itemToAdd.Manager.Id,
                StatusId = itemToAdd.Status.Id,
                StatusDate = itemToAdd.StatusDate,
                StatusDescription = itemToAdd.StatusDescription
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            itemToAdd.Id = await connection.ExecuteScalarAsync<int>(commandDefinition);

            const string sqlInsertOrderItem = @"
                INSERT INTO order_items (order_id, sku, sku_description, quantity, status_id, status_date, status_description)
                VALUES (@OrderId, @Sku, @SkuDescription, @Quantity, @StatusId, @StatusDate, @StatusDescription)
                RETURNING id;";

            foreach (var orderItem in itemToAdd.OrderItems)
            {
                var parametersOrderItem = new
                {
                    OrderId = itemToAdd.Id,
                    Sku = orderItem.Sku.Value,
                    SkuDescription = orderItem.Sku.Description,
                    Quantity = orderItem.Quantity.Value,
                    StatusId = orderItem.Status.Id,
                    StatusDate = orderItem.StatusDate,
                    StatusDescription = orderItem.StatusDescription
                };

                commandDefinition = new CommandDefinition(
                    sqlInsertOrderItem,
                    parameters: parametersOrderItem,
                    commandTimeout: Timeout,
                    cancellationToken: cancellationToken);

                orderItem.Id = await connection.ExecuteScalarAsync<int>(commandDefinition);
            }

            _changeTracker.Track(itemToAdd);

            return itemToAdd;
        }

        public async Task<MerchOrder> UpdateAsync(MerchOrder itemToUpdate,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(UpdateAsync)}").StartActive();
            
            const string sql = @"
                UPDATE merch_orders 
                SET receiver_id = @ReceiverId, manager_id = @ManagerId, 
                    status_id = @StatusId, status_date = @StatusDate, status_description = @StatusDescription
                WHERE id = @OrderId;";

            var parameters = new
            {
                ReceiverId = itemToUpdate.Receiver.Id,
                ManagerId = itemToUpdate.Manager.Id,
                StatusId = itemToUpdate.Status.Id,
                StatusDate = itemToUpdate.StatusDate,
                StatusDescription = itemToUpdate.StatusDescription,
                OrderId = itemToUpdate.Id
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            await connection.ExecuteAsync(commandDefinition);

            const string sqlUpdateOrderItem = @"
                UPDATE order_items 
                SET order_id = @OrderId, sku = @Sku, sku_description = @SkuDescription, quantity = @Quantity, 
                    status_id = @StatusId, status_date = @StatusDate, status_description = @StatusDescription
                WHERE id = @OrderItemId;";

            foreach (var orderItem in itemToUpdate.OrderItems)
            {
                var parametersOrderItem = new
                {
                    OrderId = itemToUpdate.Id,
                    Sku = orderItem.Sku.Value,
                    SkuDescription = orderItem.Sku.Description,
                    Quantity = orderItem.Quantity.Value,
                    StatusId = orderItem.Status.Id,
                    StatusDate = orderItem.StatusDate,
                    StatusDescription = orderItem.StatusDescription,
                    OrderItemId = orderItem.Id
                };

                commandDefinition = new CommandDefinition(
                    sqlUpdateOrderItem,
                    parameters: parametersOrderItem,
                    commandTimeout: Timeout,
                    cancellationToken: cancellationToken);

                await connection.ExecuteAsync(commandDefinition);
            }

            _changeTracker.Track(itemToUpdate);

            return itemToUpdate;
        }

        private async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderId(int orderId,
            CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(GetOrderItemsByOrderId)}").StartActive();
            
            const string sql = @"
                SELECT id, order_id, sku, sku_description, quantity, 
                       status_id as id, status_date, status_description
                FROM order_items
                WHERE order_id = @OrderId;";

            var parameters = new
            {
                OrderId = orderId
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var orderItems =
                await connection.QueryAsync<OrderItemQueryModel, StatusQueryModel, OrderItem>(
                    commandDefinition,
                    (orderItemModel, statusModel) =>
                    {
                        return new OrderItem(
                            orderItemModel.Id,
                            new Sku(orderItemModel.Sku, orderItemModel.SkuDescription),
                            new Quantity(orderItemModel.Quantity),
                            Enumeration.FromValue<OrderItemStatus>(statusModel.Id),
                            statusModel.StatusDate,
                            statusModel.StatusDescription);
                    });

            return orderItems;
        }

        public async Task<MerchOrder> FindByIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(FindByIdAsync)}").StartActive();
            
            var orderItems = await GetOrderItemsByOrderId(orderId, cancellationToken);
            
            _logger.LogInformation("Order items: {@items}", orderItems);
            
            const string sql = @"
                SELECT merch_orders.id, 
                       status_id, 
                       status_date, 
                       status_description,
                       receiver.id, 
                       receiver.firstname, 
                       receiver.lastname, 
                       receiver.middlename, 
                       receiver.email,
                       manager.id, 
                       manager.firstname, 
                       manager.lastname,
                       manager.middlename, 
                       manager.email
                FROM merch_orders
                INNER JOIN employees as receiver ON receiver_id = receiver.id
                INNER JOIN employees as manager ON manager_id = manager.id
                WHERE merch_orders.id = @OrderId;";

            var parameters = new
            {
                OrderId = orderId
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);

            var merchOrders =
                await connection
                    .QueryAsync<MerchOrderQueryModel, EmployeeQueryModel, EmployeeQueryModel, MerchOrder>(
                        commandDefinition, (orderModel, receiverModel, managerModel) =>
                        {
                            var receiver = new Employee(receiverModel.Id, receiverModel.FirstName,
                                receiverModel.LastName, receiverModel.MiddleName, receiverModel.Email);
                            var manager = new Employee(managerModel.Id, managerModel.FirstName,
                                managerModel.LastName, managerModel.MiddleName, managerModel.Email);
                            var order = new MerchOrder(
                                orderModel.Id,
                                receiver,
                                manager,
                                Enumeration.FromValue<OrderStatus>(orderModel.StatusId),
                                orderModel.StatusDate,
                                orderModel.StatusDescription,
                                orderItems);

                            return order;
                        });

            _logger.LogInformation("Orders: {@merchOrders}", merchOrders);
            
            var merchOrder = merchOrders.FirstOrDefault();
            if (merchOrder != null) _changeTracker.Track(merchOrder);

            return merchOrder;
        }

        public async Task<IEnumerable<OrderItem>> GetCompletedByEmployeeEmailAsync(string email,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(GetCompletedByEmployeeEmailAsync)}").StartActive();
            
            const string sql = @"
                SELECT order_items.id, 
                       order_items.order_id, 
                       order_items.sku, 
                       order_items.sku_description, 
                       order_items.quantity, 
                       order_items.status_id as id,
                       order_items.status_date, 
                       order_items.status_description
                FROM merch_orders
                INNER JOIN order_items ON merch_orders.id = order_items.order_id
                INNER JOIN employees ON employees.id = merch_orders.receiver_id 
                WHERE employees.email = @EmployeeEmail and order_items.status_id = @StatusId;";

            var parameters = new
            {
                EmployeeEmail = email,
                StatusId = OrderItemStatus.Issued.Id
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var orderItems =
                await connection.QueryAsync<OrderItemQueryModel, StatusQueryModel, OrderItem>(
                    commandDefinition,
                    (orderItemModel, statusModel) =>
                    {
                        return new OrderItem(
                            orderItemModel.Id,
                            new Sku(orderItemModel.Sku, orderItemModel.SkuDescription),
                            new Quantity(orderItemModel.Quantity),
                            Enumeration.FromValue<OrderItemStatus>(statusModel.Id),
                            statusModel.StatusDate,
                            statusModel.StatusDescription);
                    });

            return orderItems;
        }
        
        public async Task<IEnumerable<OrderItem>> GetCompletedByEmployeeIdAsync(int employeeId,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(GetCompletedByEmployeeIdAsync)}").StartActive();
            
            const string sql = @"
                SELECT order_items.id, 
                       order_items.order_id, 
                       order_items.sku, 
                       order_items.sku_description, 
                       order_items.quantity, 
                       order_items.status_id as id,
                       order_items.status_date, 
                       order_items.status_description
                FROM merch_orders
                INNER JOIN order_items ON merch_orders.id = order_items.order_id
                WHERE merch_orders.receiver_id = @EmployeeId and order_items.status_id = @StatusId;";

            var parameters = new
            {
                EmployeeId = employeeId,
                StatusId = OrderItemStatus.Issued.Id
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var orderItems =
                await connection.QueryAsync<OrderItemQueryModel, StatusQueryModel, OrderItem>(
                    commandDefinition,
                    (orderItemModel, statusModel) =>
                    {
                        return new OrderItem(
                            orderItemModel.Id,
                            new Sku(orderItemModel.Sku, orderItemModel.SkuDescription),
                            new Quantity(orderItemModel.Quantity),
                            Enumeration.FromValue<OrderItemStatus>(statusModel.Id),
                            statusModel.StatusDate,
                            statusModel.StatusDescription);
                    });

            return orderItems;
        }

        public async Task<int> GetIssuedToEmployeeQuantityLastYearBySkuAsync(int employeeId, long skuValue,
            CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(GetIssuedToEmployeeQuantityLastYearBySkuAsync)}").StartActive();
            
            const string sql = @"
                SELECT SUM(order_items.quantity)
                FROM merch_orders
                INNER JOIN order_items ON merch_orders.id = order_items.order_id
                WHERE merch_orders.receiver_id = @EmployeeId and 
                      order_items.sku = @SkuValue and 
                      order_items.status_id = @StatusId and 
                      order_items.status_date > @StatusDate;";

            var parameters = new
            {
                EmployeeId = employeeId,
                SkuValue = skuValue,
                StatusId = OrderItemStatus.Issued.Id,
                StatusDate = DateTime.UtcNow.AddYears(-1)
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);

            return await connection.ExecuteScalarAsync<int>(commandDefinition);
        }

        public async Task<IEnumerable<MerchOrder>> GetReservedByEmployeeIdAsync(int employeeId,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(GetReservedByEmployeeIdAsync)}").StartActive();
            
            const string sql = @"
                SELECT id FROM merch_orders
                WHERE receiver_id = @EmployeeId and status_id = @StatusId;";

            var parameters = new
            {
                EmployeeId = employeeId,
                StatusId = OrderStatus.Reserved.Id
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var merchOrderIds = await connection.QueryAsync<int>(commandDefinition);

            var merchOrders = new List<MerchOrder>();

            foreach (var orderId in merchOrderIds)
            {
                var order = await FindByIdAsync(orderId, cancellationToken);
                
                _logger.LogInformation("Order {@order}", order);
                
                merchOrders.Add(order);
            }

            return merchOrders;
        }
        
        public async Task<IEnumerable<MerchOrder>> GetReservedByEmployeeEmailAsync(string email,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer.BuildSpan($"{nameof(MerchOrderRepository)}.{nameof(GetReservedByEmployeeIdAsync)}").StartActive();
            
            const string sql = @"
                SELECT id FROM merch_orders
                INNER JOIN employees ON employees.id = receiver_id
                WHERE employees.email = @EmployeeEmail and status_id = @StatusId;";

            var parameters = new
            {
                EmployeeEmail = email,
                StatusId = OrderStatus.Reserved.Id
            };

            var commandDefinition = new CommandDefinition(
                sql,
                parameters: parameters,
                commandTimeout: Timeout,
                cancellationToken: cancellationToken);

            var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
            var merchOrderIds = await connection.QueryAsync<int>(commandDefinition);

            var merchOrders = new List<MerchOrder>();

            foreach (var orderId in merchOrderIds)
            {
                merchOrders.Add(await FindByIdAsync(orderId, cancellationToken));
            }

            return merchOrders;
        }
    }
}