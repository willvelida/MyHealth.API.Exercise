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
using AutoMapper;
using Microsoft.Extensions.Configuration;
using MyHealth.API.Exercise.Models;

namespace MyHealth.API.Exercise.Functions
{
    public class UpdateWeightExercise
    {
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IDateValidator _dateValidator;
        private readonly IExerciseValidator _exerciseValidator;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateWeightExercise(
            IExerciseDbService exerciseDbService,
            IDateValidator dateValidator,
            IExerciseValidator exerciseValidator,
            IServiceBusHelpers serviceBusHelpers,
            IMapper mapper,
            IConfiguration configuration)
        {
            _exerciseDbService = exerciseDbService;
            _exerciseValidator = exerciseValidator;
            _dateValidator = dateValidator;
            _serviceBusHelpers = serviceBusHelpers;
            _mapper = mapper;
            _configuration = configuration;
        }

        [FunctionName(nameof(UpdateWeightExercise))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Workout/{date}/WeightExercise/{weightExerciseId}")] HttpRequest req,
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

                var weightExerciseToUpdate = _exerciseValidator.GetWeightExerciseById(workout, weightExerciseId);
                if (weightExerciseToUpdate is null)
                {
                    result = new NoContentResult();
                    return result;
                }

                string messageRequest = await new StreamReader(req.Body).ReadToEndAsync();
                var weightRequest = JsonConvert.DeserializeObject<WeightExerciseRequestDto>(messageRequest);
                var updatedWeightExercise = _mapper.Map(weightRequest, weightExerciseToUpdate);

                var updatedWorkout = _exerciseValidator.UpdateWeightExerciseInExerciseEnvelope(workout, updatedWeightExercise);
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
