using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using OzonEdu.MerchandiseService.Domain.SeedWork;

namespace OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate
{
    public sealed class Employee : Entity, IAggregateRoot
    {
        public Employee(int id, string firstName, string lastName, string middleName, string email)
        {
            Id = id;
            PersonName = PersonName.Create(firstName, lastName, middleName);
            Email = Email.Create(email);
        }

        public PersonName PersonName { get; }
        public Email Email { get; }
    }
}