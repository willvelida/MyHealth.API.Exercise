using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using MyHealth.API.Exercise.Services;
using MyHealth.API.Exercise.UnitTests.TestExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.UnitTests.ServiceTests
{
    public class ExerciseDbServiceShould
    {
        private readonly Mock<CosmosClient> _cosmosClientMock;
        private readonly Mock<Container> _containerMock;
        private readonly Mock<IConfiguration> _configMock;

        private ExerciseDbService _sut;

        public ExerciseDbServiceShould()
        {
            _cosmosClientMock = new Mock<CosmosClient>();
            _containerMock = new Mock<Container>();
            _cosmosClientMock.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_containerMock.Object);
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["BookDatabaseName"]).Returns("db");
            _configMock.Setup(x => x["BookContainerName"]).Returns("col");

            _sut = new ExerciseDbService(_configMock.Object, _cosmosClientMock.Object);
        }

        [Fact]
        public async Task CreateWorkoutItemSuccessfully()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.SetupCreateItemAsync<mdl.ExerciseEnvelope>();

            Func<Task> createWorkoutAction = async () => await _sut.CreateWorkout(testWorkout);

            await createWorkoutAction.Should().NotThrowAsync<Exception>();
            _containerMock.Verify(x => x.CreateItemAsync(
                It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DeleteWorkoutItemSuccessfully()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.SetupDeleteItemAsync<mdl.ExerciseEnvelope>();

            Func<Task> deleteWorkoutAction = async () => await _sut.DeleteWorkout(testWorkout);

            await deleteWorkoutAction.Should().NotThrowAsync<Exception>();
            _containerMock.Verify(x => x.DeleteItemAsync<mdl.ExerciseEnvelope>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetAllWorkoutItemsSuccessfully()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();
            List<mdl.ExerciseEnvelope> testExercises = new List<mdl.ExerciseEnvelope>();
            testExercises.Add(testWorkout);

            _containerMock.SetupItemQueryIteratorMock(testExercises);
            _containerMock.SetupItemQueryIteratorMock(new List<int> { 1 });

            var response = await _sut.GetAllWorkouts();

            using (new AssertionScope())
            {
                response[0].Date.Should().Be(testWorkout.Date);
                response[0].DocumentType.Should().Be(testWorkout.DocumentType);
            }

            Assert.Equal(testExercises.Count, response.Count);
        }

        [Fact]
        public async Task GetWorkoutByDateSuccessfully()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();
            List<mdl.ExerciseEnvelope> testExercises = new List<mdl.ExerciseEnvelope>();
            testExercises.Add(testWorkout);

            _containerMock.SetupItemQueryIteratorMock(testExercises);
            _containerMock.SetupItemQueryIteratorMock(new List<int> { 1 });

            var response = await _sut.GetWorkoutByDate(testWorkout.Date);

            using (new AssertionScope())
            {
                response.Date.Should().Be(testWorkout.Date);
                response.DocumentType.Should().Be(testWorkout.DocumentType);
            }
        }

        [Fact]
        public async Task UpdateWorkoutItemSuccessfully()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.SetupReplaceItemAsync<mdl.ExerciseEnvelope>();

            await _sut.UpdateWorkout(testWorkout);
            _containerMock.Verify(x => x.ReplaceItemAsync(
                It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()));
        }
    }
}
