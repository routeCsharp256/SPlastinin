using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregatesModel.MerchOrderAggregate
{
    public sealed class MerchOrderMocks
    {
        public Employee Employee => Employee.Create(1, "Ivan", "Ivanov", "", "test@fake.mail");
        public OrderItem OrderItem => OrderItem.Create(123456789, "Faked hoody XXXL", 2);
        public Employee Manager => Employee.Create(2, "Petr", "Petrov", "Petrovich", "petr@fake.mail");
        public DateTime UtcNow => new DateTime(2020, 6, 10, 23, 23, 23, 233);

        public MerchOrderMocks()
        {
        }

        public MerchOrder CreatedOrder
        {
            get => new MerchOrder(Employee, OrderItem);
        }

        public MerchOrder AssignedOrder
        {
            get
            {
                var order = CreatedOrder;
                order.AssignTo(Manager, UtcNow);
                return order;
            }
        }

        public MerchOrder InProgressOrder
        {
            get
            {
                var order = AssignedOrder;
                order.SetInProgressStatus(UtcNow);
                return order;
            }
        }

        public MerchOrder DeferredOrder
        {
            get
            {
                var order = InProgressOrder;
                order.SetDeferredStatus(UtcNow);
                return order;
            }
        }

        public MerchOrder ReservedOrder
        {
            get
            {
                var order = InProgressOrder;
                order.SetReservedStatus(UtcNow);
                return order;
            }
        }

        public MerchOrder CompletedOrder
        {
            get
            {
                var order = ReservedOrder;
                order.SetCompletedStatus(UtcNow);
                return order;
            }
        }

        public MerchOrder CanceledOrder
        {
            get
            {
                var order = CreatedOrder;
                order.SetCanceledStatus("Test", UtcNow);
                return order;
            }
        }
    }
}