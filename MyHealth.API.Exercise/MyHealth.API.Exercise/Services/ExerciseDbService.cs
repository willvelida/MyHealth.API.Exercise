using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Services
{
    public class ExerciseDbService : IExerciseDbService
    {
        private readonly IConfiguration _configuration;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public ExerciseDbService(
            IConfiguration configuration,
            CosmosClient cosmosClient)
        {
            _configuration = configuration;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_configuration["DatabaseName"], _configuration["ContainerName"]);
        }

        public Task CreateCardioExercise()
        {
            throw new NotImplementedException();
        }

        public Task CreateWeightExercise()
        {
            throw new NotImplementedException();
        }

        public async Task CreateWorkout(mdl.ExerciseEnvelope exerciseEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.CreateItemAsync(
                    exerciseEnvelope,
                    new PartitionKey(exerciseEnvelope.DocumentType),
                    itemRequestOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task DeleteCardioExercise()
        {
            throw new NotImplementedException();
        }

        public Task DeleteWeightExercise()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteWorkout(mdl.ExerciseEnvelope exerciseEnvelope)
        {
            try
            {
                await _container.DeleteItemAsync<mdl.ExerciseEnvelope>(
                    exerciseEnvelope.Id,
                    new PartitionKey(exerciseEnvelope.DocumentType));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<mdl.ExerciseEnvelope>> GetAllWorkouts()
        {
            try
            {
                QueryDefinition query = new QueryDefinition("SELECT * FROM Records c WHERE c.DocumentType = 'Exercise'");

                List<mdl.ExerciseEnvelope> exercises = new List<mdl.ExerciseEnvelope>();

                FeedIterator<mdl.ExerciseEnvelope> feedIterator = _container.GetItemQueryIterator<mdl.ExerciseEnvelope>();

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<mdl.ExerciseEnvelope> exerciseQueryResponse = await feedIterator.ReadNextAsync();
                    exercises.AddRange(exerciseQueryResponse.Resource);
                }

                return exercises;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<mdl.ExerciseEnvelope> GetWorkoutByDate(string date)
        {
            try
            {
                QueryDefinition query = new QueryDefinition("SELECT * FROM Records c WHERE c.DocumentType = 'Exercise' AND c.Date = @date")
                    .WithParameter("@date", date);

                List<mdl.ExerciseEnvelope> exercises = new List<mdl.ExerciseEnvelope>();

                FeedIterator<mdl.ExerciseEnvelope> feedIterator = _container.GetItemQueryIterator<mdl.ExerciseEnvelope>(query);

                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<mdl.ExerciseEnvelope> exerciseQueryResponse = await feedIterator.ReadNextAsync();
                    exercises.AddRange(exerciseQueryResponse.Resource);
                }

                return exercises.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;   
            }
        }

        public Task ReadCardioExercise()
        {
            throw new NotImplementedException();
        }

        public Task ReadWeightExercise()
        {
            throw new NotImplementedException();
        }

        public Task UpdateCardioExercise()
        {
            throw new NotImplementedException();
        }

        public Task UpdateWeightExercise()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateWorkout(mdl.ExerciseEnvelope exerciseEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                };

                await _container.ReplaceItemAsync(
                    exerciseEnvelope,
                    exerciseEnvelope.Id,
                    new PartitionKey(exerciseEnvelope.DocumentType),
                    itemRequestOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
