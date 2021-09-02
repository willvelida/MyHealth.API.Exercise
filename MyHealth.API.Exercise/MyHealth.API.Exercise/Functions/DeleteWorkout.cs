using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.API.Exercise.Services;
using MyHealth.Common;
using System;
using System.Threading.Tasks;

namespace MyHealth.API.Exercise.Functions
{
    public class DeleteWorkout
    {
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public DeleteWorkout(
            IExerciseDbService exerciseDbService,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration)
        {
            _exerciseDbService = exerciseDbService;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName(nameof(DeleteWorkout))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Workout/{id}")] HttpRequest req,
            ILogger log,
            string id)
        {
            IActionResult result;

            try
            {
                // Get the workout
                var workout = await _exerciseDbService.GetWorkoutById(id);
                if (workout is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                await _exerciseDbService.DeleteWorkout(workout);

                result = new NoContentResult();
            }
            catch (Exception ex)
            {
                log.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                await _serviceBusHelpers.SendMessageToQueue(_configuration["ExceptionQueue"], ex);
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
