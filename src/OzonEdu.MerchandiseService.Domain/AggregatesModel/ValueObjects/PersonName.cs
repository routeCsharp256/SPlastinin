using System;
using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects
{
    public sealed class PersonName : ValueObject
    {
        private PersonName(string firstName, string lastName, string middleName)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            MiddleName = middleName;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string MiddleName { get; }

        public static PersonName Create(string firstName, string lastName, string middleName = null)
        {
            return new PersonName(firstName, lastName, middleName);
        }

        public override string ToString() => $"{FirstName} {LastName}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return LastName;
            yield return FirstName;
            yield return MiddleName;
        }
    }
}