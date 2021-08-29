using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MyHealth.API.Exercise.Services;
using MyHealth.API.Exercise.Validators;
using MyHealth.Common;
using Microsoft.Extensions.Configuration;

namespace MyHealth.API.Exercise.Functions
{
    public class GetWorkoutByDate
    {
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IDateValidator _dateValidator;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public GetWorkoutByDate(
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

        [FunctionName("GetWorkoutByDate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Workout/{date}")] HttpRequest req,
            ILogger log,
            string date)
        {
            IActionResult result = null;

            try
            {
                bool isDateValid = _dateValidator.IsDateValid(date);
                if (isDateValid is false)
                {
                    result = new BadRequestResult();
                    return result;
                }

                var exerciseResponse = await _exerciseDbService.GetWorkoutByDate(date);
                if (exerciseResponse is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(exerciseResponse);
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
