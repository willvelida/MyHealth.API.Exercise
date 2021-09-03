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
    public class DeleteWeightWorkoutById
    {
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IDateValidator _dateValidator;
        private readonly IExerciseValidator _exerciseValidator;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public DeleteWeightWorkoutById(
            IExerciseDbService exerciseDbService,
            IDateValidator dateValidator,
            IExerciseValidator exerciseValidator,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration)
        {
            _exerciseDbService = exerciseDbService;
            _exerciseValidator = exerciseValidator;
            _dateValidator = dateValidator;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName(nameof(DeleteWeightWorkoutById))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Workout/{date}/WeightExercise/{weightExerciseById}")] HttpRequest req,
            ILogger log,
            string date,
            string weightExerciseId)
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

                var weightExerciseToDelete = _exerciseValidator.GetWeightExerciseById(workout, weightExerciseId);
                if (weightExerciseToDelete is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                var updatedWorkout = _exerciseValidator.RemoveWeightExerciseFromExerciseEnvelope(workout, weightExerciseToDelete);
                if (updatedWorkout is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                await _exerciseDbService.UpdateWorkout(updatedWorkout);
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
