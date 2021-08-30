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
using Microsoft.Extensions.Configuration;
using mdl = MyHealth.Common.Models;
using System.Collections.Generic;
using MyHealth.Common;

namespace MyHealth.API.Exercise.Functions
{
    public class GetAllWorkouts
    {
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IExerciseValidator _exerciseValidator;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;

        public GetAllWorkouts(
            IExerciseDbService exerciseDbService,
            IExerciseValidator exerciseValidator,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration)
        {
            _exerciseDbService = exerciseDbService;
            _exerciseValidator = exerciseValidator;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
        }

        [FunctionName(nameof(GetAllWorkouts))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Workout")] HttpRequest req,
            ILogger log)
        {
            IActionResult result;

            try
            {
                var exerciseResponses = await _exerciseDbService.GetAllWorkouts();

                if (exerciseResponses is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(exerciseResponses);
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
