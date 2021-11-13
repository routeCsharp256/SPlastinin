using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects
{
    public sealed class Sku : ValueObject
    {
        public Sku(long sku, string description)
        {
            Value = sku;
            Description = description;
        }

        public long Value { get; }
        public string Description { get; }

        public override string ToString()
        {
            return $"{Description} (Sku:{Value})";
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Description;
        }
    }
}