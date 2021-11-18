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
            var firstName = "Ivan";
            var lastName = "Ivanov";
            var middleName = "Ivanovich";
            var email = "ivan@fake.mail";

            //Act 
            var employee = new Employee(id, firstName, lastName, middleName, email);

            //Assert
            Assert.NotNull(employee);
        }
    }
}