using System;
using System.Collections.Generic;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Validators
{
    public class ExerciseValidator : IExerciseValidator
    {
        public mdl.ExerciseEnvelope CreateValidExerciseEnvelope()
        {
            mdl.ExerciseEnvelope exerciseEnvelope = new mdl.ExerciseEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                WeightExercises = new List<mdl.WeightExercise>(),
                CardioExercise = new List<mdl.CardioExercise>(),
                DocumentType = "Exercise",
                Date = DateTime.Today.ToString("yyyy-MM-dd")
            };

            return exerciseEnvelope;
        }
    }
}
