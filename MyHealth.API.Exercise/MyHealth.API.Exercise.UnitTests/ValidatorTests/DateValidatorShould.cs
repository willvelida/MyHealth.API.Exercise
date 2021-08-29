using MyHealth.API.Exercise.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MyHealth.API.Exercise.UnitTests.ValidatorTests
{
    public class DateValidatorShould
    {
        private DateValidator _sut;

        public DateValidatorShould()
        {
            _sut = new DateValidator();
        }

        [Theory]
        [InlineData("20111-01-01")]
        [InlineData("2021-111-01")]
        [InlineData("2021-01-111")]
        public void ReturnFalseIfDateIsInvalid(string date)
        {
            string testDate = date;

            var response = _sut.IsDateValid(date);

            Assert.False(response);
        }

        [Fact]
        public void ReturnTrueIfDateIsValid()
        {
            string testDate = "2021-01-01";

            var response = _sut.IsDateValid(testDate);

            Assert.True(response);
        }
    }
}
