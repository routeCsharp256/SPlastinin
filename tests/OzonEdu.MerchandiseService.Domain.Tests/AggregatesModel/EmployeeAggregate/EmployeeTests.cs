using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregatesModel.EmployeeAggregate
{
    public class EmployeeTests
    {
        [Fact]
        public void CreateEmployee_Test()
        {
            //Arrange    
            int id = 1;
            var personName = PersonName.Create("Ivan", "Ivanov");
            var email = Email.Create("ivan@fake.mail");
            
            //Act 
            var employee1 = Employee.Create(id, personName.FirstName, personName.LastName, personName.MiddleName,
                email.Value);
            var employee2 = Employee.Create(id, personName, email);

            //Assert
            Assert.NotNull(employee1);
            Assert.NotNull(employee2);
            Assert.Throws<ArgumentNullException>(() => Employee.Create(0, null, email));
            Assert.Throws<ArgumentNullException>(() => Employee.Create(0, personName, null));
        }
    }
}