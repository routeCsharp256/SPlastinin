using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregatesModel.MerchOrderAggregate
{
    public class MerchOrderTests
    {
        private readonly MerchOrderMocks _mocks;

        public MerchOrderTests()
        {
            _mocks = new MerchOrderMocks();
        }

        [Fact]
        public void CreateMerchOrder_Test()
        {
            //Arrange    
            var employee = _mocks.Employee;

            //Act 
            var order1 = new MerchOrder(employee);

            //Assert
            Assert.NotNull(order1);
            Assert.Throws<ArgumentNullException>(() => new MerchOrder(null));
        }

        [Fact]
        public void AddOrderItem_Test()
        {
            //Arrange    
            var orderDraft = _mocks.DraftOrder;
            var createdOrder = _mocks.CreatedOrder;
            var orderItem = _mocks.OrderItem;
            
            var expectedValueForDraft = 1;
            
            //Act
            orderDraft.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description, orderItem.Quantity.Value, _mocks.UtcNow);
            createdOrder.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description, orderItem.Quantity.Value, _mocks.UtcNow);

            //Assert
            Assert.Equal(expectedValueForDraft, orderDraft.OrderItems.Count);
            Assert.True(createdOrder.OrderItems.Count > expectedValueForDraft);
            Assert.Throws<MerchOrderAggregateException>(() =>
                _mocks.AssignedOrder.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description,
                    orderItem.Quantity.Value, _mocks.UtcNow));
            Assert.Throws<MerchOrderAggregateException>(() =>
                _mocks.InProgressOrder.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description,
                    orderItem.Quantity.Value, _mocks.UtcNow));
            Assert.Throws<MerchOrderAggregateException>(() =>
                _mocks.DeferredOrder.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description,
                    orderItem.Quantity.Value, _mocks.UtcNow));
            Assert.Throws<MerchOrderAggregateException>(() =>
                _mocks.ReservedOrder.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description,
                    orderItem.Quantity.Value, _mocks.UtcNow));
            Assert.Throws<MerchOrderAggregateException>(() =>
                _mocks.CompletedOrder.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description,
                    orderItem.Quantity.Value, _mocks.UtcNow));
            Assert.Throws<MerchOrderAggregateException>(() =>
                _mocks.CanceledOrder.AddOrderItem(orderItem.Sku.Value, orderItem.Sku.Description,
                    orderItem.Quantity.Value, _mocks.UtcNow));
        }

        [Fact]
        public void AssignTo_Test()
        {
            //Arrange    
            var utcNow = _mocks.UtcNow;
            var orderWithoutManager = _mocks.CreatedOrder;
            var orderWithManager = _mocks.AssignedOrder;
            var manager = Employee.Create(3, "Sergei", "Sergeev", "Sergeevich", "sergei@fake.mail");

            //Act
            orderWithoutManager.AssignTo(manager, utcNow);
            orderWithManager.AssignTo(manager, utcNow);

            //Assert
            Assert.True(manager == orderWithoutManager.Manager);
            Assert.True(manager == orderWithManager.Manager);
            Assert.Throws<ArgumentNullException>(() => orderWithoutManager.AssignTo(null, utcNow));
        }

        [Fact]
        public void SetAssignedStatus_Test()
        {
            //Arrange    
            var utcNow = _mocks.UtcNow;
            var orderWithManager = _mocks.AssignedOrder;
            var orderWithoutManager = _mocks.CreatedOrder;

            //Act - Assert
            Assert.True(orderWithManager.Status.Equals(OrderStatus.Assigned));
            Assert.Throws<MerchOrderAggregateException>(() => orderWithoutManager.SetAssignedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CompletedOrder.SetAssignedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CanceledOrder.SetAssignedStatus(utcNow));
        }

        [Fact]
        public void SetInProgressStatus_Test()
        {
            //Arrange    
            var utcNow = _mocks.UtcNow;
            var order = _mocks.AssignedOrder;

            //Act
            order.SetInProgressStatus(utcNow);

            //Assert
            Assert.True(order.Status.Equals(OrderStatus.InProgress));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CreatedOrder.SetInProgressStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CanceledOrder.SetInProgressStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CompletedOrder.SetInProgressStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.ReservedOrder.SetInProgressStatus(utcNow));
        }

        [Fact]
        public void SetDeferredStatus_Test()
        {
            //Arrange    
            var utcNow = _mocks.UtcNow;
            var order = _mocks.InProgressOrder;

            //Act
            order.SetDeferredStatus(utcNow);

            //Assert
            Assert.True(order.Status.Equals(OrderStatus.Deferred));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CreatedOrder.SetDeferredStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CanceledOrder.SetDeferredStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CompletedOrder.SetDeferredStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.AssignedOrder.SetDeferredStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.ReservedOrder.SetDeferredStatus(utcNow));
        }

        [Fact]
        public void SetReservedStatus_Test()
        {
            //Arrange    
            var utcNow = _mocks.UtcNow;
            var order = _mocks.InProgressOrder;

            //Act
            order.SetReservedStatus(utcNow);

            //Assert
            Assert.True(order.Status.Equals(OrderStatus.Reserved));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CreatedOrder.SetReservedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CanceledOrder.SetReservedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CompletedOrder.SetReservedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.AssignedOrder.SetReservedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.DeferredOrder.SetReservedStatus(utcNow));
        }

        [Fact]
        public void SetCompletedStatus_Test()
        {
            //Arrange    
            var utcNow = _mocks.UtcNow;
            var order = _mocks.ReservedOrder;

            //Act
            order.SetCompletedStatus(utcNow);

            //Assert
            Assert.True(order.Status.Equals(OrderStatus.Completed));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CreatedOrder.SetCompletedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CanceledOrder.SetCompletedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.InProgressOrder.SetCompletedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.AssignedOrder.SetCompletedStatus(utcNow));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.DeferredOrder.SetCompletedStatus(utcNow));
        }

        [Fact]
        public void SetCanceledStatus_Test()
        {
            //Arrange    
            var utcNow = _mocks.UtcNow;
            var order = _mocks.CreatedOrder;

            //Act
            order.SetCanceledStatus("Test", utcNow);

            //Assert
            Assert.True(order.Status.Equals(OrderStatus.Canceled));
            Assert.Throws<MerchOrderAggregateException>(() => _mocks.CompletedOrder.SetCanceledStatus("Test", utcNow));
        }

        [Fact]
        public void AssignToRaisesNewEvent_Test()
        {
            //Arrange
            var order = _mocks.CreatedOrder;
            var expectedResult = (order.DomainEvents?.Count ?? 0) + 1;

            //Act 
            order.AssignTo(_mocks.Manager, _mocks.UtcNow);

            //Assert
            Assert.Equal(order.DomainEvents.Count, expectedResult);
        }
    }
}