using System;
using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects
{
    public sealed class OrderItemWithIssueDate : ValueObject
    {
        public OrderItemWithIssueDate(OrderItem item, DateTime issueDate)
        {
            Item = item;
            IssueDate = issueDate;
        }

        public OrderItem Item { get; }
        public DateTime IssueDate { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Item;
            yield return IssueDate;
        }
    }
}