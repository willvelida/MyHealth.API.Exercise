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
    public class GetAllWeightWorkoutsByDate
    {
        private readonly IDateValidator _dateValidator;
        private readonly IExerciseValidator _exerciseValidator;
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public GetAllWeightWorkoutsByDate(
            IExerciseDbService exerciseDbService,
            IExerciseValidator exerciseValidator,
            IDateValidator dateValidator,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration)
        {
            _exerciseDbService = exerciseDbService;
            _exerciseValidator = exerciseValidator;
            _dateValidator = dateValidator;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName(nameof(GetAllWeightWorkoutsByDate))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Workout/{date}/WeightExercise")] HttpRequest req,
            ILogger log,
            string date)
        {
            IActionResult result;

            try
            {
                bool isDateValid = _dateValidator.IsDateValid(date);
                if (isDateValid is false)
                {
                    result = new BadRequestResult();
                    return result;
                }

                var workout = await _exerciseDbService.GetWorkoutByDate(date);
                if (workout is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                var weightWorkouts = _exerciseValidator.ReturnWeightExercisesInExerciseEnvelope(workout);
                if (weightWorkouts is null || weightWorkouts.Count == 0)
                {
                    result = new OkResult();
                    return result;
                }

                result = new OkObjectResult(weightWorkouts);
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
