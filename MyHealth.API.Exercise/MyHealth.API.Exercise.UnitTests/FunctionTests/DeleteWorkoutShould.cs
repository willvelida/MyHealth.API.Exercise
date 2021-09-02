using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyHealth.API.Exercise.Functions;
using MyHealth.API.Exercise.Services;
using MyHealth.Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.UnitTests.FunctionTests
{
    public class DeleteWorkoutShould
    {
        private Mock<IExerciseDbService> _exerciseDbServiceMock;
        private Mock<IServiceBusHelpers> _serviceBusHelpersMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IConfiguration> _configMock;

        private DeleteWorkout _func;

        public DeleteWorkoutShould()
        {
            _exerciseDbServiceMock = new Mock<IExerciseDbService>();
            _serviceBusHelpersMock = new Mock<IServiceBusHelpers>();
            _loggerMock = new Mock<ILogger>();
            _httpRequestMock = new Mock<HttpRequest>();
            _configMock = new Mock<IConfiguration>();

            _func = new DeleteWorkout(
                _exerciseDbServiceMock.Object,
                _serviceBusHelpersMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task ReturnNotFoundResultWhenGetWorkoutByIdReturnsNull()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _exerciseDbServiceMock.Setup(x => x.GetWorkoutById(It.IsAny<string>())).Returns(Task.FromResult<mdl.ExerciseEnvelope>(null));

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Id);

            // Assert
            Assert.Equal(typeof(NotFoundResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(404, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ReturnNoContentResultWhenWorkoutHasBeenSuccessfullyDeleted()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _exerciseDbServiceMock.Setup(x => x.GetWorkoutById(It.IsAny<string>())).ReturnsAsync(workout);
            _exerciseDbServiceMock.Setup(x => x.DeleteWorkout(It.IsAny<mdl.ExerciseEnvelope>())).Returns(Task.CompletedTask);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Id);

            // Assert
            Assert.Equal(typeof(NoContentResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(204, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task Throw500WhenInternalServerErrorHasOccurred()
        {
            // Arrange
            var fixture = new Fixture();
            var workout = fixture.Create<mdl.ExerciseEnvelope>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);

            _exerciseDbServiceMock.Setup(x => x.GetWorkoutById(It.IsAny<string>())).ReturnsAsync(workout);
            _exerciseDbServiceMock.Setup(x => x.DeleteWorkout(It.IsAny<mdl.ExerciseEnvelope>())).ThrowsAsync(new Exception());

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object, workout.Id);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
