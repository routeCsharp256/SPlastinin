using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.MerchOrderAggregate;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregatesModel.MerchOrderAggregate
{
    public sealed class MerchOrderMocks
    {
        public Employee Employee => new Employee(1, "Ivan", "Ivanov", "", "test@fake.mail");
        public Employee Manager => new Employee(2, "Petr", "Petrov", "Petrovich", "petr@fake.mail");
        public DateTime UtcNow => new DateTime(2020, 6, 10, 23, 23, 23, 233);
        public OrderItem OrderItem => OrderItem.Create(123456789, "Faked hoody XXXL", 2, DateTime.UtcNow);

        public MerchOrderMocks()
        {
        }

        public MerchOrder DraftOrder
        {
            get => new MerchOrder(Employee);
        }

        public MerchOrder CreatedOrder
        {
            get
            {
                var order = new MerchOrder(Employee);
                order.AddOrderItem(123456789, "Faked hoody XXXL", 2, DateTime.UtcNow);
                return order;
            }
        }

        public MerchOrder InProgressOrder
        {
            get
            {
                var order = CreatedOrder;
                order.AssignTo(Manager, UtcNow);
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