using MyHealth.Common.Models;
using System;
using System.Collections.Generic;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Validators
{
    public class ExerciseValidator : IExerciseValidator
    {
        public CardioExercise CreateValidCardioExerciseObject(CardioExercise cardioExercise)
        {
            mdl.CardioExercise cardioExerciseToPersist = new CardioExercise
            {
                CardioExerciseId = Guid.NewGuid().ToString(),
                Name = cardioExercise.Name,
                DurationInMinutes = cardioExercise.DurationInMinutes
            };

            return cardioExerciseToPersist;
        }

        public mdl.ExerciseEnvelope CreateValidExerciseEnvelope()
        {
            mdl.ExerciseEnvelope exerciseEnvelope = new mdl.ExerciseEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                WeightExercises = new List<mdl.WeightExercise>(),
                CardioExercises = new List<mdl.CardioExercise>(),
                DocumentType = "Exercise",
                Date = DateTime.Today.ToString("yyyy-MM-dd")
            };

            return exerciseEnvelope;
        }

        public WeightExercise CreateValidWeightExerciseObject(WeightExercise weightExercise)
        {
            mdl.WeightExercise weightExerciseToPersist = new WeightExercise
            {
                WeightExerciseId = Guid.NewGuid().ToString(),
                Name = weightExercise.Name,
                Weight = weightExercise.Weight,
                Reps = weightExercise.Reps,
                Notes = weightExercise.Notes
            };

            return weightExerciseToPersist;
        }

        public List<CardioExercise> ReturnCardioExercisesInExerciseEnvelope(ExerciseEnvelope exerciseEnvelope)
        {
            List<CardioExercise> cardioExercisesToReturn = new List<CardioExercise>();

            if (exerciseEnvelope.CardioExercises is null)
                return null;

            foreach (var cardio in exerciseEnvelope.CardioExercises)
            {
                cardioExercisesToReturn.Add(cardio);
            }

            return cardioExercisesToReturn;
        }
    }
}
