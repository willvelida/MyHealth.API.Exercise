using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MyHealth.API.Exercise.Validators;
using System;
using System.Collections.Generic;
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
        public void ReturnNullWhenThereAreNoCardioExercisesInExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            exerciseEnvelope.CardioExercises = null;

            var expectedCardioExercises = _sut.ReturnCardioExercisesInExerciseEnvelope(exerciseEnvelope);

            expectedCardioExercises.Should().BeNull();
        }

        [Fact]
        public void ReturnWeightExercisesInExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();

            var expectedWeightExercises = _sut.ReturnWeightExercisesInExerciseEnvelope(exerciseEnvelope);

            using (new AssertionScope())
            {
                Assert.Equal(expectedWeightExercises.Count, exerciseEnvelope.WeightExercises.Count);
                expectedWeightExercises[0].Reps.Should().Be(exerciseEnvelope.WeightExercises[0].Reps);
                expectedWeightExercises[0].Name.Should().Be(exerciseEnvelope.WeightExercises[0].Name);
                expectedWeightExercises[0].Weight.Should().Be(exerciseEnvelope.WeightExercises[0].Weight);
                expectedWeightExercises[0].Notes.Should().Be(exerciseEnvelope.WeightExercises[0].Notes);
                expectedWeightExercises[0].WeightExerciseId.Should().Be(exerciseEnvelope.WeightExercises[0].WeightExerciseId);
            }
        }

        [Fact]
        public void ReturnNullWhenThereAreNoWeightExercisesInExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            exerciseEnvelope.WeightExercises = null;

            var expectedWeightExercises = _sut.ReturnWeightExercisesInExerciseEnvelope(exerciseEnvelope);

            expectedWeightExercises.Should().BeNull();
        }

        [Fact]
        public void ReturnWeightExceriseByIdInExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var actualExercise = exerciseEnvelope.WeightExercises[0];

            var expectedWeight = _sut.GetWeightExerciseById(exerciseEnvelope.WeightExercises, actualExercise.WeightExerciseId);

            using (new AssertionScope())
            {
                expectedWeight.Name.Should().Be(actualExercise.Name);
                expectedWeight.Notes.Should().Be(actualExercise.Notes);
                expectedWeight.WeightExerciseId.Should().Be(actualExercise.WeightExerciseId);
                expectedWeight.Reps.Should().Be(actualExercise.Reps);
                expectedWeight.Weight.Should().Be(actualExercise.Weight);
            }
        }

        [Fact]
        public void ReturnNullWhenThereAreNoWeightExercisesWithProvidedIdInWeightExerciseList()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();

            var expectedWeight = _sut.GetWeightExerciseById(exerciseEnvelope.WeightExercises, "1");

            expectedWeight.Should().BeNull();
        }

        [Fact]
        public void ReturnCardioExerciseByIdInExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var actualExercise = exerciseEnvelope.CardioExercises[0];

            var expectedCardio = _sut.GetCardioExerciseById(exerciseEnvelope.CardioExercises, actualExercise.CardioExerciseId);

            using (new AssertionScope())
            {
                expectedCardio.CardioExerciseId.Should().Be(actualExercise.CardioExerciseId);
                expectedCardio.Name.Should().Be(actualExercise.Name);
                expectedCardio.DurationInMinutes.Should().Be(actualExercise.DurationInMinutes);
            }
        }

        [Fact]
        public void ReturnCardioExerciseByIdInExerciseEnvelopePassingThroughExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var actualExercise = exerciseEnvelope.CardioExercises[0];

            var expectedCardio = _sut.GetCardioExerciseById(exerciseEnvelope, actualExercise.CardioExerciseId);

            using (new AssertionScope())
            {
                expectedCardio.CardioExerciseId.Should().Be(actualExercise.CardioExerciseId);
                expectedCardio.Name.Should().Be(actualExercise.Name);
                expectedCardio.DurationInMinutes.Should().Be(actualExercise.DurationInMinutes);
            }
        }

        [Fact]
        public void ReturnNullWhenThereAreNoCardioExercisesWithProvidedIdInCardioExerciseList()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();

            var expectedCardio = _sut.GetCardioExerciseById(exerciseEnvelope.CardioExercises, "1");

            expectedCardio.Should().BeNull();
        }

        [Fact]
        public void ReturnNullWhenThereAreNoCardioExercisesWithProvidedIdInCardioExerciseListWhenPassingThroughExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            exerciseEnvelope.CardioExercises = null;

            var expectedCardio = _sut.GetCardioExerciseById(exerciseEnvelope, "1");

            expectedCardio.Should().BeNull();
        }

        [Fact]
        public void ReturnNullWhenThereAreZeroCardioExercisesWithProvidedIdInCardioExerciseListWhenPassingThroughExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            exerciseEnvelope.CardioExercises = new List<mdl.CardioExercise>();

            var expectedCardio = _sut.GetCardioExerciseById(exerciseEnvelope, "1");

            expectedCardio.Should().BeNull();
        }

        [Fact]
        public void UpdateCardioExerciseInExerciseEnvelope()
        {
            var exerciseEnvelope = new mdl.ExerciseEnvelope
            {
                CardioExercises = new List<mdl.CardioExercise>()
                {
                    new mdl.CardioExercise
                    {
                        CardioExerciseId = Guid.NewGuid().ToString(),
                        Name = "Rowing",
                        DurationInMinutes = 30
                    }
                }
            };

            var cardioEnvelope = new mdl.CardioExercise
            {
                CardioExerciseId = exerciseEnvelope.CardioExercises[0].CardioExerciseId,
                Name = "Jogging",
                DurationInMinutes = 60
            };

            var expectedUpdatedExercise = _sut.UpdateCardioExerciseInExerciseEnvelope(exerciseEnvelope, cardioEnvelope);

            using (new AssertionScope())
            {
                expectedUpdatedExercise.CardioExercises[0].CardioExerciseId.Should().Be(cardioEnvelope.CardioExerciseId);
                expectedUpdatedExercise.CardioExercises[0].Name.Should().Be(cardioEnvelope.Name);
                expectedUpdatedExercise.CardioExercises[0].DurationInMinutes.Should().Be(cardioEnvelope.DurationInMinutes);
            }
        }

        [Fact]
        public void ReturnNullWhenCardioToRemoveDoesNotExist()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var cardioEnvelope = fixture.Create<mdl.CardioExercise>();
            cardioEnvelope.CardioExerciseId = "1";

            var expectedUpdatedWorkout = _sut.UpdateCardioExerciseInExerciseEnvelope(exerciseEnvelope, cardioEnvelope);

            expectedUpdatedWorkout.Should().BeNull();
        }

        [Fact]
        public void ReturnWeightExercieByIdInExerciseEnvelopeWhenPassingThroughExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var actualExercise = exerciseEnvelope.WeightExercises[0];

            var expectedWeight = _sut.GetWeightExerciseById(exerciseEnvelope, actualExercise.WeightExerciseId);

            using (new AssertionScope())
            {
                expectedWeight.Name.Should().Be(actualExercise.Name);
                expectedWeight.Notes.Should().Be(actualExercise.Notes);
                expectedWeight.WeightExerciseId.Should().Be(actualExercise.WeightExerciseId);
                expectedWeight.Reps.Should().Be(actualExercise.Reps);
                expectedWeight.Weight.Should().Be(actualExercise.Weight);
            }
        }

        [Fact]
        public void ReturnNullWhenThereAreNoWeightExercisesWithProvidedIdInWeightExerciseListWhenPassingThroughExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            exerciseEnvelope.WeightExercises = null;

            var expectedWeight = _sut.GetWeightExerciseById(exerciseEnvelope, "1");

            expectedWeight.Should().BeNull();
        }

        [Fact]
        public void ReturnNullWhenThereAreZeroWeightExercisesWithProvidedIdInWeightExerciseListWhenPassingThroughExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            exerciseEnvelope.WeightExercises = new List<mdl.WeightExercise>();

            var expectedWeight = _sut.GetWeightExerciseById(exerciseEnvelope, "1");

            expectedWeight.Should().BeNull();
        }

        [Fact]
        public void UpdateWeightExerciseInExerciseEnvelope()
        {
            var exerciseEnvelope = new mdl.ExerciseEnvelope
            {
                WeightExercises = new List<mdl.WeightExercise>()
                {
                    new mdl.WeightExercise
                    {
                        WeightExerciseId = Guid.NewGuid().ToString(),
                        Name = "Bench",
                        Weight = 90.0,
                        Reps = 8,
                        Notes = "This was fun"
                    }
                }
            };

            var weightEnvelope = new mdl.WeightExercise
            {
                WeightExerciseId = exerciseEnvelope.WeightExercises[0].WeightExerciseId,
                Name = "Bench Press",
                Weight = 100.0,
                Reps = 6,
                Notes = "This was hard"
            };

            var expectedUpdatedWorkout = _sut.UpdateWeightExerciseInExerciseEnvelope(exerciseEnvelope, weightEnvelope);

            using (new AssertionScope())
            {
                expectedUpdatedWorkout.WeightExercises[0].Name.Should().Be(weightEnvelope.Name);
                expectedUpdatedWorkout.WeightExercises[0].Notes.Should().Be(weightEnvelope.Notes);
                expectedUpdatedWorkout.WeightExercises[0].WeightExerciseId.Should().Be(weightEnvelope.WeightExerciseId);
                expectedUpdatedWorkout.WeightExercises[0].Reps.Should().Be(weightEnvelope.Reps);
                expectedUpdatedWorkout.WeightExercises[0].Weight.Should().Be(weightEnvelope.Weight);
            }
        }

        [Fact]
        public void ReturnNullWhenWeightToRemoveDoesNotExist()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var weightEnvelope = fixture.Create<mdl.WeightExercise>();
            weightEnvelope.WeightExerciseId = "1";

            var expectedUpdatedWorkout = _sut.UpdateWeightExerciseInExerciseEnvelope(exerciseEnvelope, weightEnvelope);

            expectedUpdatedWorkout.Should().BeNull();
        }

        [Fact]
        public void RemoveCardioExerciseWhenCallingRemoveCardioExerciseFromExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var cardioToRemove = exerciseEnvelope.CardioExercises[0];

            var expectedUpdatedWorkout = _sut.RemoveCardioExerciseFromExerciseEnvelope(exerciseEnvelope, cardioToRemove);

            Assert.Equal(2, expectedUpdatedWorkout.CardioExercises.Count);
        }

        [Fact]
        public void ReturnNullWhenCardioExerciseIsNullWhenRemovingCardioExerciseFromExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();

            var expectedUpdatedWorkout = _sut.RemoveCardioExerciseFromExerciseEnvelope(exerciseEnvelope, new mdl.CardioExercise());

            expectedUpdatedWorkout.Should().BeNull();
        }

        [Fact]
        public void RemoveWeightExerciseWhenCallingRemoveWeightExerciseFromExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();
            var weightToRemove = exerciseEnvelope.WeightExercises[0];

            var expectedUpdatedWorkout = _sut.RemoveWeightExerciseFromExerciseEnvelope(exerciseEnvelope, weightToRemove);

            Assert.Equal(2, exerciseEnvelope.WeightExercises.Count);
        }

        [Fact]
        public void ReturnNullWhenWeightExerciseIsNullWhenRemovingWeightExerciseFromExerciseEnvelope()
        {
            var fixture = new Fixture();
            var exerciseEnvelope = fixture.Create<mdl.ExerciseEnvelope>();

            var expectedUpdatedWorkout = _sut.RemoveWeightExerciseFromExerciseEnvelope(exerciseEnvelope, new mdl.WeightExercise());

            expectedUpdatedWorkout.Should().BeNull();
        }
    }
}
