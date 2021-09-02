using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using MyHealth.API.Exercise.Mappers;
using MyHealth.API.Exercise.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.UnitTests.MapperTests
{
    public class MapWeightExerciseRequestDtoToWeightExerciseShould
    {
        private IMapper _sut;

        public MapWeightExerciseRequestDtoToWeightExerciseShould()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapWeightExerciseRequestDtoToWeightExercise>());
            _sut = config.CreateMapper();
        }

        [Fact]
        public void MapWeightExerciseRequestDtoToWeightExercise()
        {
            var weightExercise = new mdl.WeightExercise
            {
                WeightExerciseId = Guid.NewGuid().ToString(),
                Name = "Bench Press",
                Weight = 90.0,
                Reps = 8,
                Notes = "This was fun"
            };

            var weightRequest = new WeightExerciseRequestDto
            {
                Name = "Barbell Bench Press",
                Weight = 100.0,
                Reps = 6,
                Notes = "This was tough"
            };

            var expectedRequest = _sut.Map(weightRequest, weightExercise);

            using (new AssertionScope())
            {
                expectedRequest.WeightExerciseId.Should().Be(weightExercise.WeightExerciseId);
                expectedRequest.Name.Should().Be(weightRequest.Name);
                expectedRequest.Weight.Should().Be(weightRequest.Weight);
                expectedRequest.Reps.Should().Be(weightRequest.Reps);
                expectedRequest.Notes.Should().Be(weightRequest.Notes);
            }
        }
    }
}
