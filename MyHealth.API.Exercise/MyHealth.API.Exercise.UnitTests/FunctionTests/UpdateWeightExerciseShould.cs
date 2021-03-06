using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyHealth.API.Exercise.Functions;
using MyHealth.API.Exercise.Services;
using MyHealth.API.Exercise.Validators;
using MyHealth.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.UnitTests.FunctionTests
{
    public class UpdateWeightExerciseShould
    {
        private Mock<IExerciseDbService> _exerciseDbServiceMock;
        private Mock<IDateValidator> _dateValidatorMock;
        private Mock<IExerciseValidator> _exerciseValidatorMock;
        private Mock<IServiceBusHelpers> _serviceBusHelpersMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IConfiguration> _configMock;

        private UpdateWeightExercise _func;

        public UpdateWeightExerciseShould()
        {
            _exerciseDbServiceMock = new Mock<IExerciseDbService>();
            _dateValidatorMock = new Mock<IDateValidator>();
            _exerciseValidatorMock = new Mock<IExerciseValidator>();
            _serviceBusHelpersMock = new Mock<IServiceBusHelpers>();
            _loggerMock = new Mock<ILogger>();
            _mapperMock = new Mock<IMapper>();
            _httpRequestMock = new Mock<HttpRequest>();
            _configMock = new Mock<IConfiguration>();

            _func = new UpdateWeightExercise(
                _exerciseDbServiceMock.Object,
                _dateValidatorMock.Object,
                _exerciseValidatorMock.Object,
                _serviceBusHelpersMock.Object,
                _mapperMock.Object,
                _configMock.Object);
        }

        [Theory]
        [InlineData("2020-12-100")]
        [InlineData("2020-100-12")]
        [InlineData("20201-12-11")]
        public async Task ThrowBadRequestResultWhenWorkoutDateRequestIsInvalid(string invalidDateInput)
        {
            // Arrange
            var workouts = new List<mdl.ExerciseEnvelope>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workouts));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(invalidDateInput)).Returns(false);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, invalidDateInput, "1");

            // Assert
            Assert.Equal(typeof(BadRequestResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(400, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ThrowNotFoundResultWhenWorkoutIsNull()
        {
            // Arrange
            var workouts = new List<mdl.ExerciseEnvelope>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workouts));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(It.IsAny<string>())).Returns(true);
            _exerciseDbServiceMock.Setup(x => x.GetWorkoutByDate(It.IsAny<string>())).Returns(Task.FromResult<mdl.ExerciseEnvelope>(null));

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, "2019-12-31", "1");

            // Assert
            Assert.Equal(typeof(NotFoundResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(404, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ReturnNoContentResultWhenWorkoutIsFoundByHasNoWeightWorkoutsLogged()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            workout.Date = "2021-08-29";
            workout.WeightExercises = null;
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(It.IsAny<string>())).Returns(true);
            _exerciseDbServiceMock.Setup(x => x.GetWorkoutByDate(It.IsAny<string>())).ReturnsAsync(workout);
            _exerciseValidatorMock.Setup(x => x.ReturnWeightExercisesInExerciseEnvelope(It.IsAny<mdl.ExerciseEnvelope>())).Returns(workout.WeightExercises);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Date, "1");

            // Assert
            Assert.Equal(typeof(NoContentResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(204, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ReturnNotFoundResultWhenUpdatedWorkoutObjectCantBeFound()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            workout.Date = "2021-08-29";
            var updatedWorkout = fixture.Create<mdl.ExerciseEnvelope>();
            updatedWorkout = null;
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(It.IsAny<string>())).Returns(true);
            _exerciseDbServiceMock.Setup(x => x.GetWorkoutByDate(It.IsAny<string>())).ReturnsAsync(workout);
            _exerciseValidatorMock.Setup(x => x.GetWeightExerciseById(It.IsAny<mdl.ExerciseEnvelope>(), It.IsAny<string>())).Returns(workout.WeightExercises[0]);
            _exerciseValidatorMock.Setup(x => x.UpdateWeightExerciseInExerciseEnvelope(It.IsAny<mdl.ExerciseEnvelope>(), It.IsAny<mdl.WeightExercise>())).Returns(updatedWorkout);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Date, workout.CardioExercises[0].CardioExerciseId);

            // Assert
            Assert.Equal(typeof(NotFoundResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(404, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ReturnNoContentResultWhenWeightExerciseHasBeenSuccessfullyUpdated()
        {
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            workout.Date = "2021-08-29";
            var updatedWorkout = fixture.Create<mdl.ExerciseEnvelope>();
            updatedWorkout.WeightExercises[0].WeightExerciseId = workout.WeightExercises[0].WeightExerciseId;
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(It.IsAny<string>())).Returns(true);
            _exerciseDbServiceMock.Setup(x => x.GetWorkoutByDate(It.IsAny<string>())).ReturnsAsync(workout);
            _exerciseValidatorMock.Setup(x => x.GetWeightExerciseById(It.IsAny<mdl.ExerciseEnvelope>(), It.IsAny<string>())).Returns(workout.WeightExercises[0]);
            _exerciseValidatorMock.Setup(x => x.UpdateWeightExerciseInExerciseEnvelope(It.IsAny<mdl.ExerciseEnvelope>(), It.IsAny<mdl.WeightExercise>())).Returns(updatedWorkout);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Date, workout.CardioExercises[0].CardioExerciseId);

            // Assert
            Assert.Equal(typeof(NoContentResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(204, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task Throw500InternalServerError()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            workout.Date = "2021-08-29";
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(It.IsAny<string>())).Returns(true);
            _exerciseDbServiceMock.Setup(x => x.GetWorkoutByDate(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Date, "1");

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
