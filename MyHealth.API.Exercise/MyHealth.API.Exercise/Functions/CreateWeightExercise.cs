using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.API.Exercise.Services;
using MyHealth.API.Exercise.Validators;
using MyHealth.Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Functions
{
    public class CreateWeightExercise
    {
        private readonly IDateValidator _dateValidator;
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public CreateWeightExercise(
            IExerciseDbService exerciseDbService,
            IDateValidator dateValidator,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration)
        {
            _exerciseDbService = exerciseDbService;
            _dateValidator = dateValidator;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName("CreateWeightExercise")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Workout/{date}/WeightExercise")] HttpRequest req,
            ILogger log,
            string date)
        {
            IActionResult result;

            try
            {
                // validate the date
                bool isDateValid = _dateValidator.IsDateValid(date);
                if (isDateValid is false)
                {
                    result = new BadRequestResult();
                    return result;
                }

                // get the workout out envelope
                var workout = await _exerciseDbService.GetWorkoutByDate(date);
                if (workout is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                // parse the incoming request to a Weight Exercise
                string messageRequest = await new StreamReader(req.Body).ReadToEndAsync();
                var weightExercise = JsonConvert.DeserializeObject<mdl.WeightExercise>(messageRequest);

                // Add weight exercise to workout
                await _exerciseDbService.CreateWeightExercise(workout, weightExercise);

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
