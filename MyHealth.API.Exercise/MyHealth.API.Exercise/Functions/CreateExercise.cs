using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyHealth.API.Exercise.Functions
{
    public static class CreateExercise
    {
        [Function("CreateExercise")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workout/{workoutId}/exercise")] HttpRequestData req,
            FunctionContext executionContext,
            string workoutId)
        {
            var logger = executionContext.GetLogger("CreateExercise");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
