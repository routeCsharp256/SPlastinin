using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate
{
    public sealed class Employee : Entity, IAggregateRoot
    {
        private Employee(int id, PersonName personName, Email email)
        {
            Id = id;
            PersonName = personName ?? throw new ArgumentNullException(nameof(personName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        public PersonName PersonName { get; }
        public Email Email { get; }

        public static Employee Create(int id, PersonName personName, Email email)
        {
            return new Employee(id, personName, email);
        }

        public static Employee Create(int id, string firstName, string lastName, string middleName, string email)
        {
            return new Employee(id,
                PersonName.Create(firstName, lastName, middleName),
                Email.Create(email));
        }
    }
}