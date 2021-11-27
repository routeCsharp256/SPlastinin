using System;
using OzonEdu.MerchandiseService.Domain.AggregatesModel.ValueObjects;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregatesModel.ValueObjects
{
    public class QuantityTests
    {
        [Fact]
        public void CreateQuantity_Test()
        {
            // Arrange
            const int value = 10;

            // Act
            var quantity = new Quantity(value);

            // Assert
            Assert.NotNull(quantity);
        }

        [Fact]
        public void ShouldThrowExceptionWhenArgumentLessOrEqualsZero_Test()
        {
            // Arrange
            const int value1 = 0;
            const int value2 = -145;

            // Act - Assert
            Assert.Throws<ArgumentException>(() => new Quantity(value1));
            Assert.Throws<ArgumentException>(() => new Quantity(value2));
        }
        
        [Fact]
        public void IsValueValid_Test()
        {
            //Arrange    
            int validValue = 100;
            int invalidValue = -100;
            
            //Act 
            var validQuantity = Quantity.IsValueValid(validValue);
            var invalidQuantity = Quantity.IsValueValid(invalidValue);

            //Assert
            Assert.True(validQuantity, "Must be true, because value is valid");
            Assert.False(invalidQuantity, "Must be false, because value is invalid");
        }
    }
}