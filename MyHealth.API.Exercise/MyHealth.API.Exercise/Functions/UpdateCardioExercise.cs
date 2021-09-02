using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.API.Exercise.Models;
using MyHealth.API.Exercise.Services;
using MyHealth.API.Exercise.Validators;
using MyHealth.Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyHealth.API.Exercise.Functions
{
    public class UpdateCardioExercise
    {
        private readonly IExerciseDbService _exerciseDbService;
        private readonly IDateValidator _dateValidator;
        private readonly IExerciseValidator _exerciseValidator;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpdateCardioExercise(
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

        [FunctionName(nameof(UpdateCardioExercise))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Workout/{date}/CardioExercise/{cardioExerciseId}")] HttpRequest req,
            ILogger log,
            string date,
            string cardioExerciseId)
        {
            IActionResult result;

            try
            {
                // validate date
                bool isDateValid = _dateValidator.IsDateValid(date);
                if (isDateValid is false)
                {
                    result = new BadRequestResult();
                    return result;
                }

                // get workout document
                var workout = await _exerciseDbService.GetWorkoutByDate(date);
                if (workout is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                // get cardio exercise
                var cardioExerciseToUpdate = _exerciseValidator.GetCardioExerciseById(workout, cardioExerciseId);
                if (cardioExerciseToUpdate is null)
                {
                    result = new NoContentResult();
                    return result;
                }

                // parse incoming cardioExercise
                string messageRequest = await new StreamReader(req.Body).ReadToEndAsync();
                var cardioRequest = JsonConvert.DeserializeObject<CardioExerciseRequestDto>(messageRequest);
                var updatedCardioExercise = _mapper.Map(cardioRequest, cardioExerciseToUpdate);

                // update the cardio exercise in the workout envelope
                var updatedWorkout = _exerciseValidator.UpdateCardioExerciseInExerciseEnvelope(workout, updatedCardioExercise);
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
