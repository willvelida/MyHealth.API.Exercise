using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.API.Exercise.Services;
using MyHealth.API.Exercise.Validators;
using MyHealth.Common;
using System;
using System.Threading.Tasks;

namespace MyHealth.API.Exercise.Functions
{
    public class CreateWorkout
    {
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IExerciseValidator _exerciseValidator;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public CreateWorkout(
            IExerciseDbService exerciseDbService,
            IExerciseValidator exerciseValidator,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration
            )
        {
            _exerciseDbService = exerciseDbService;
            _exerciseValidator = exerciseValidator;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName(nameof(CreateWorkout))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Workout")] HttpRequest req,
            ILogger log)
        {
            IActionResult result = null;

            try
            {
                var workout = _exerciseValidator.CreateValidExerciseEnvelope();

                await _exerciseDbService.CreateWorkout(workout);

                result = new OkResult();
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
