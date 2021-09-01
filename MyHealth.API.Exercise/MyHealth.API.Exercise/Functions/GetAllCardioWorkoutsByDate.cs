using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MyHealth.API.Exercise.Validators;
using MyHealth.API.Exercise.Services;
using MyHealth.Common;
using Microsoft.Extensions.Configuration;

namespace MyHealth.API.Exercise.Functions
{
    public class GetAllCardioWorkoutsByDate
    {
        private readonly IDateValidator _dateValidator;
        private readonly IExerciseValidator _exerciseValidator;
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public GetAllCardioWorkoutsByDate(
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

        [FunctionName(nameof(GetAllCardioWorkoutsByDate))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Workout/{date}/CardioExercise")] HttpRequest req,
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

                // for each cardio workout log in exercise envelope, add to list to return
                var cardioWorkouts = _exerciseValidator.ReturnCardioExercisesInExerciseEnvelope(workout);
                if (cardioWorkouts is null || cardioWorkouts.Count == 0)
                {
                    result = new OkResult();
                    return result;
                }

                result = new OkObjectResult(cardioWorkouts);
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
