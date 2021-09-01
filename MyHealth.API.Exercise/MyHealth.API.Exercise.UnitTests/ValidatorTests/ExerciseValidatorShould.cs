﻿using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MyHealth.API.Exercise.Validators;
using System;
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

        [Fact]
        public void CreateValidCardioExerciseObject()
        {
            var incomingCardioRequest = new mdl.CardioExercise
            {
                Name = "Jogging",
                DurationInMinutes = 60
            };

            var expectedCardioExercise = _sut.CreateValidCardioExerciseObject(incomingCardioRequest);

            using (new AssertionScope())
            {
                expectedCardioExercise.CardioExerciseId.Should().BeOfType<string>();
                expectedCardioExercise.Name.Should().Be(incomingCardioRequest.Name);
                expectedCardioExercise.DurationInMinutes.Should().Be(incomingCardioRequest.DurationInMinutes);
            }
        }

        [Fact]
        public void CreateValidWeightExerciseObject()
        {
            var incomingWeightExerciseRequest = new mdl.WeightExercise
            {
                Name = "Bench Press",
                Weight = 90.0,
                Reps = 8,
                Notes = "This is a test"
            };

            var expectedWeightExercise = _sut.CreateValidWeightExerciseObject(incomingWeightExerciseRequest);

            using (new AssertionScope())
            {
                expectedWeightExercise.WeightExerciseId.Should().BeOfType<string>();
                expectedWeightExercise.Name.Should().Be(incomingWeightExerciseRequest.Name);
                expectedWeightExercise.Weight.Should().Be(incomingWeightExerciseRequest.Weight);
                expectedWeightExercise.Reps.Should().Be(incomingWeightExerciseRequest.Reps);
                expectedWeightExercise.Notes.Should().Be(incomingWeightExerciseRequest.Notes);
            }
        }

        [Fact]
        public void ReturnCardioExercisesInExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();

            var expectedCardioExercises = _sut.ReturnCardioExercisesInExerciseEnvelope(exerciseEnvelope);

            using (new AssertionScope())
            {
                Assert.Equal(expectedCardioExercises.Count, exerciseEnvelope.CardioExercises.Count);
                expectedCardioExercises[0].CardioExerciseId.Should().Be(exerciseEnvelope.CardioExercises[0].CardioExerciseId);
                expectedCardioExercises[0].Name.Should().Be(exerciseEnvelope.CardioExercises[0].Name);
                expectedCardioExercises[0].DurationInMinutes.Should().Be(exerciseEnvelope.CardioExercises[0].DurationInMinutes);
            }
        }

        [Fact]
        public void ThrowExceptionWhenThereAreNoCardioExercisesInExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            exerciseEnvelope.CardioExercises = null;

            var expectedCardioExercises = _sut.ReturnCardioExercisesInExerciseEnvelope(exerciseEnvelope);

            expectedCardioExercises.Should().BeNull();
        }
    }
}
