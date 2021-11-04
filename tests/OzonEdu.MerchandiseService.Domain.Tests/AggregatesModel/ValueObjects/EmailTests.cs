using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregatesModel.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void CreateEmail_Test()
        {
            //Arrange    
            string emailAddress = "ivan@fake.mail";
            
            //Act 
            var email = Email.Create(emailAddress);

            //Assert
            Assert.NotNull(email);
        }
        
        [Fact]
        public void IsValidEmail_Test()
        {
            //Arrange    
            string validEmailAddress = "ivan@fake.mail";
            string invalidEmailAddress = "ivan@ivanov@fake.mail";
            
            //Act 
            var validEmail = Email.IsEmailValid(validEmailAddress);
            var invalidEmail = Email.IsEmailValid(invalidEmailAddress);

            //Assert
            Assert.True(validEmail, "Must be true, because email is valid");
            Assert.False(invalidEmail, "Must be false, because email is invalid");
        }
    }
}