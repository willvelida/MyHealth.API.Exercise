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
        public async Task ThrowExceptionWhenCreateWorkoutFails()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.Setup(x => x.CreateItemAsync(
                It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).Throws(new Exception());

            Func<Task> createWorkoutAction = async () => await _sut.CreateWorkout(testWorkout);
            await createWorkoutAction.Should().ThrowAsync<Exception>();
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
        public async Task ThrowExceptionWhenDeleteWorkoutFails()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.Setup(x => x.DeleteItemAsync<mdl.ExerciseEnvelope>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).Throws(new Exception());

            Func<Task> deleteWorkoutAction = async () => await _sut.DeleteWorkout(testWorkout);
            await deleteWorkoutAction.Should().ThrowAsync<Exception>();
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
        public async Task ThrowExceptionWhenGetAllWorkoutItemsFails()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();
            List<mdl.ExerciseEnvelope> testExercises = new List<mdl.ExerciseEnvelope>();
            testExercises.Add(testWorkout);

            _containerMock.Setup(x => x.GetItemQueryIterator<mdl.ExerciseEnvelope>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>())).Throws(new Exception());

            Func<Task> exerciseDbAction = async () => await _sut.GetAllWorkouts();

            await exerciseDbAction.Should().ThrowAsync<Exception>();
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
        public async Task ThrowExceptionWhenGetWorkoutByDateFails()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();
            List<mdl.ExerciseEnvelope> testExercises = new List<mdl.ExerciseEnvelope>();
            testExercises.Add(testWorkout);

            _containerMock.Setup(x => x.GetItemQueryIterator<mdl.ExerciseEnvelope>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>())).Throws(new Exception());

            Func<Task> exerciseDbAction = async () => await _sut.GetWorkoutByDate(testWorkout.Date);

            await exerciseDbAction.Should().ThrowAsync<Exception>();
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
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenUpdateWorkoutFails()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.Setup(x => x.ReplaceItemAsync(It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            Func<Task> exerciseDbAction = async () => await _sut.UpdateWorkout(testWorkout);
            await exerciseDbAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateWeightExerciseSuccessfully()
        {
            var fixture = new Fixture();
            var testWeightExercise = fixture.Create<mdl.WeightExercise>();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.SetupReplaceItemAsync<mdl.ExerciseEnvelope>();

            await _sut.CreateWeightExercise(testWorkout, testWeightExercise);

            _containerMock.Verify(x => x.ReplaceItemAsync(
                It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateWeightExerciseFails()
        {
            var fixture = new Fixture();
            var testWeightExercise = fixture.Create<mdl.WeightExercise>();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.Setup(x => x.ReplaceItemAsync(It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            Func<Task> exerciseDbAction = async () => await _sut.CreateWeightExercise(testWorkout, testWeightExercise);

            await exerciseDbAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateCardioExerciseSuccessfully()
        {
            var fixture = new Fixture();
            var testCardioExercise = fixture.Create<mdl.CardioExercise>();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.SetupReplaceItemAsync<mdl.ExerciseEnvelope>();

            await _sut.CreateCardioExercise(testWorkout, testCardioExercise);

            _containerMock.Verify(x => x.ReplaceItemAsync(
                It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateCardioExerciseFails()
        {
            var fixture = new Fixture();
            var testCardioExercise = fixture.Create<mdl.CardioExercise>();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.Setup(x => x.ReplaceItemAsync(It.IsAny<mdl.ExerciseEnvelope>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            Func<Task> exerciseDbAction = async () => await _sut.CreateCardioExercise(testWorkout, testCardioExercise);

            await exerciseDbAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task GetWorkoutItemByIdSuccessfully()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.SetupReadItemAsync<mdl.ExerciseEnvelope>(testWorkout);

            await _sut.GetWorkoutById(testWorkout.Id);

            _containerMock.Verify(x => x.ReadItemAsync<mdl.ExerciseEnvelope>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenGetWorkoutItemByIdFails()
        {
            var fixture = new Fixture();
            var testWorkout = fixture.Create<mdl.ExerciseEnvelope>();

            _containerMock.Setup(x => x.ReadItemAsync<mdl.ExerciseEnvelope>(
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            Func<Task> exerciseDbAction = async () => await _sut.GetWorkoutById(testWorkout.Id);

            await exerciseDbAction.Should().ThrowAsync<Exception>();
        }
    }
}
