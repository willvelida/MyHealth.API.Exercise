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
    public class GetAllWorkoutsShould
    {
        private Mock<IExerciseDbService> _exerciseDbServiceMock;
        private Mock<IExerciseValidator> _exerciseValidatorMock;
        private Mock<IServiceBusHelpers> _serviceBusHelpersMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IConfiguration> _configMock;

        private GetAllWorkouts _func;

        public GetAllWorkoutsShould()
        {
            _exerciseDbServiceMock = new Mock<IExerciseDbService>();
            _exerciseValidatorMock = new Mock<IExerciseValidator>();
            _serviceBusHelpersMock = new Mock<IServiceBusHelpers>();
            _loggerMock = new Mock<ILogger>();
            _httpRequestMock = new Mock<HttpRequest>();
            _configMock = new Mock<IConfiguration>();

            _func = new GetAllWorkouts(
                _exerciseDbServiceMock.Object,
                _exerciseValidatorMock.Object,
                _serviceBusHelpersMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task ReturnOkObjectResultWhenExerciseRecordsAreFound()
        {
            // Arrange
            var workouts = new List<mdl.ExerciseEnvelope>();
            var fixture = new Fixture();
            var exerise = fixture.Create<mdl.ExerciseEnvelope>();
            workouts.Add(exerise);

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workouts));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);
            _exerciseDbServiceMock.Setup(x => x.GetAllWorkouts()).ReturnsAsync(workouts);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            Assert.Equal(typeof(OkObjectResult), response.GetType());
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task ReturnOkObjectResultWhenNoExerciseRecordsAreFound()
        {
            // Arrange
            var workouts = new List<mdl.ExerciseEnvelope>();
            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workouts));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);
            _exerciseDbServiceMock.Setup(x => x.GetAllWorkouts()).ReturnsAsync(workouts);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            Assert.Equal(typeof(OkObjectResult), response.GetType());
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task Throw404WhenNoWorkoutsAreFound()
        {
            // Arrange
            MemoryStream memoryStream = new MemoryStream();
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);
            _exerciseDbServiceMock.Setup(x => x.GetAllWorkouts()).Returns(Task.FromResult<List<mdl.ExerciseEnvelope>>(null));

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            Assert.Equal(typeof(NotFoundResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(404, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task Throw500OnInternalServerError()
        {
            var workouts = new List<mdl.ExerciseEnvelope>();
            var fixture = new Fixture();
            var exerise = fixture.Create<mdl.ExerciseEnvelope>();
            workouts.Add(exerise);

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(workouts));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);
            _exerciseDbServiceMock.Setup(x => x.GetAllWorkouts()).ThrowsAsync(new Exception());

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCodeResult.StatusCode);
            _serviceBusHelpersMock.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
