using System;
using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects
{
    public sealed class Quantity : ValueObject
    {
        public Quantity(int value)
        {
            if(!IsValueValid(value))
                throw new ArgumentException($"Argument {nameof(value)} must be greater than zero");

            Value = value;
        }

        public static bool IsValueValid(int value)
        {
            return value > 0;
        }
        
        public int Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}