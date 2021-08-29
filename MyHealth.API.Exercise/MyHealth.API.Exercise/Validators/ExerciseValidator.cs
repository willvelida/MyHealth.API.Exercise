using mdl = MyHealth.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
