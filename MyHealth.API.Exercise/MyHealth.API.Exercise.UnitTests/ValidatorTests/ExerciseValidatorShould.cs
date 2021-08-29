using FluentAssertions;
using FluentAssertions.Execution;
using MyHealth.API.Exercise.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.UnitTests.ValidatorTests
{
    public class ExerciseValidatorShould
    {
        private ExerciseValidator _sut;

        public ExerciseValidatorShould()
        {
            _sut = new ExerciseValidator();
        }

        [Fact]
        public void CreateValidExerciseEnvelopeObject()
        {
            var expectedExerciseEnvelope = _sut.CreateValidExerciseEnvelope();
            
            using (new AssertionScope())
            {
                expectedExerciseEnvelope.Should().BeOfType<mdl.ExerciseEnvelope>();
                expectedExerciseEnvelope.DocumentType.Should().Be("Exercise");
            }
        }
    }
}
