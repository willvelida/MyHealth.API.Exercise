using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using MyHealth.API.Exercise.Mappers;
using MyHealth.API.Exercise.Models;
using System;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.UnitTests.MapperTests
{
    public class MapCardioExerciseRequestDtoToCardioExerciseShould
    {
        private IMapper _sut;

        public MapCardioExerciseRequestDtoToCardioExerciseShould()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MapCardioExerciseRequestDtoToCardioExercise>());
            _sut = config.CreateMapper();
        }

        [Fact]
        public void MapCardioExerciseRequestDtoToCardioExercise()
        {
            var cardioExercise = new mdl.CardioExercise
            {
                CardioExerciseId = Guid.NewGuid().ToString(),
                Name = "Jogging",
                DurationInMinutes = 60
            };

            var cardioRequest = new CardioExerciseRequestDto
            {
                Name = "Jogging",
                DurationInMinutes = 30
            };

            var expectedRequest = _sut.Map(cardioRequest, cardioExercise);

            using (new AssertionScope())
            {
                expectedRequest.CardioExerciseId.Should().Be(cardioExercise.CardioExerciseId);
                expectedRequest.Name.Should().Be(cardioRequest.Name);
                expectedRequest.DurationInMinutes.Should().Be(cardioRequest.DurationInMinutes);
            }
        }
    }
}
