using AutoFixture;
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
    public class GetAllCardioWorkoutsByDateShould
    {
        private Mock<IExerciseDbService> _exerciseDbServiceMock;
        private Mock<IDateValidator> _dateValidatorMock;
        private Mock<IExerciseValidator> _exerciseValidatorMock;
        private Mock<IServiceBusHelpers> _serviceBusHelpersMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IConfiguration> _configMock;

        private GetAllCardioWorkoutsByDate _func;

        public GetAllCardioWorkoutsByDateShould()
        {
            _exerciseDbServiceMock = new Mock<IExerciseDbService>();
            _dateValidatorMock = new Mock<IDateValidator>();
            _exerciseValidatorMock = new Mock<IExerciseValidator>();
            _serviceBusHelpersMock = new Mock<IServiceBusHelpers>();
            _loggerMock = new Mock<ILogger>();
            _httpRequestMock = new Mock<HttpRequest>();
            _configMock = new Mock<IConfiguration>();

            _func = new GetAllCardioWorkoutsByDate(
                _exerciseDbServiceMock.Object,
                _exerciseValidatorMock.Object,
                _dateValidatorMock.Object,
                _serviceBusHelpersMock.Object,
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
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, invalidDateInput);

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
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, "2019-12-31");

            // Assert
            Assert.Equal(typeof(NotFoundResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(404, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ReturnOkObjectResultWhenWorkoutIsFound()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            workout.Date = "2021-08-29";
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(It.IsAny<string>())).Returns(true);
            _exerciseDbServiceMock.Setup(x => x.GetWorkoutByDate(It.IsAny<string>())).ReturnsAsync(workout);
            _exerciseValidatorMock.Setup(x => x.ReturnCardioExercisesInExerciseEnvelope(It.IsAny<mdl.ExerciseEnvelope>())).Returns(workout.CardioExercises);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Date);

            // Assert
            Assert.Equal(typeof(OkObjectResult), response.GetType());
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ReturnOkResultWhenWorkoutIsFoundByHasNoCardioWorkoutsLogged()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            workout.Date = "2021-08-29";
            workout.CardioExercises = null;
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _dateValidatorMock.Setup(x => x.IsDateValid(It.IsAny<string>())).Returns(true);
            _exerciseDbServiceMock.Setup(x => x.GetWorkoutByDate(It.IsAny<string>())).ReturnsAsync(workout);
            _exerciseValidatorMock.Setup(x => x.ReturnCardioExercisesInExerciseEnvelope(It.IsAny<mdl.ExerciseEnvelope>())).Returns(workout.CardioExercises);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Date);

            // Assert
            Assert.Equal(typeof(OkResult), response.GetType());
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
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Date);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
