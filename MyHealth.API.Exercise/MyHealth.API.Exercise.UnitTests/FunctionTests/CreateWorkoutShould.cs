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
    public class CreateWorkoutShould
    {
        private Mock<IExerciseDbService> _exerciseDbServiceMock;
        private Mock<IExerciseValidator> _exerciseValidatorMock;
        private Mock<IServiceBusHelpers> _serviceBusHelpersMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IConfiguration> _configMock;

        private CreateWorkout _func;

        public CreateWorkoutShould()
        {
            _exerciseDbServiceMock = new Mock<IExerciseDbService>();
            _exerciseValidatorMock = new Mock<IExerciseValidator>();
            _serviceBusHelpersMock = new Mock<IServiceBusHelpers>();
            _loggerMock = new Mock<ILogger>();
            _httpRequestMock = new Mock<HttpRequest>();
            _configMock = new Mock<IConfiguration>();

            _func = new CreateWorkout(
                _exerciseDbServiceMock.Object,
                _exerciseValidatorMock.Object,
                _serviceBusHelpersMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task CreateNewWorkout()
        {
            // Arrange
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(testWorkout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);
            _exerciseDbServiceMock.Setup(x => x.CreateWorkout(testWorkout)).Returns(Task.CompletedTask);

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            Assert.Equal(typeof(OkResult), response.GetType());
            var responseAsStatusCode = (StatusCodeResult)response;
            Assert.Equal(200, responseAsStatusCode.StatusCode);
            _exerciseDbServiceMock.Verify(x => x.CreateWorkout(It.IsAny<mdl.ExerciseEnvelope>()), Times.Once);
        }

        [Fact]
        public async Task Throw500OnInternalServerError()
        {
            // Arrange
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(testWorkout));
            MemoryStream memoryStream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(memoryStream);
            _exerciseValidatorMock.Setup(x => x.CreateValidExerciseEnvelope()).Throws(new Exception());

            // Act
            var response = await _func.Run(_httpRequestMock.Object, _loggerMock.Object);

            // Assert
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseAsStatusCodeResult = (StatusCodeResult)response;
            Assert.Equal(500, responseAsStatusCodeResult.StatusCode);
        }
    }
}
