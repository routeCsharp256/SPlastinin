using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregatesModel.ValueObjects
{
    public class PersonNameTests
    {
        [Fact]
        public void CreatePersonName_Test()
        {
            //Arrange    
            const string firstName = "Ivan";
            const string lastName = "Ivanov";
            const string middleName = "Ivanovich";

            //Act 
            var personName1 = PersonName.Create(firstName, lastName, middleName);
            var personName2 = PersonName.Create(firstName, lastName);
            
            //Assert
            Assert.NotNull(personName1);
            Assert.NotNull(personName2);
            Assert.Throws<ArgumentNullException>(() => PersonName.Create(null, lastName));
            Assert.Throws<ArgumentNullException>(() => PersonName.Create(firstName, null));
        }
    }
}