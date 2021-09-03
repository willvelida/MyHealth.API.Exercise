using MyHealth.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public CardioExercise GetCardioExerciseById(List<CardioExercise> cardioExercises, string cardioExerciseId)
        {
            var cardioExercise = cardioExercises.Where(x => x.CardioExerciseId == cardioExerciseId).FirstOrDefault();

            return cardioExercise;
        }

        public CardioExercise GetCardioExerciseById(ExerciseEnvelope exercise, string cardioExerciseId)
        {
            var cardioExercises = exercise.CardioExercises;
            if (cardioExercises is null)
                return null;

            if (cardioExercises.Count == 0)
                return null;

            var cardioExercise = cardioExercises.Where(x => x.CardioExerciseId == cardioExerciseId).FirstOrDefault();

            return cardioExercise;
        }

        public WeightExercise GetWeightExerciseById(List<WeightExercise> weightExercises, string weightExerciseId)
        {
            var weightExercise = weightExercises.Where(x => x.WeightExerciseId == weightExerciseId).FirstOrDefault();

            return weightExercise;
        }

        public WeightExercise GetWeightExerciseById(ExerciseEnvelope exercise, string weightExerciseId)
        {
            var weightExercises = exercise.WeightExercises;
            if (weightExercises is null)
                return null;

            if (weightExercises.Count == 0)
                return null;

            var weightExercise = weightExercises.Where(x => x.WeightExerciseId == weightExerciseId).FirstOrDefault();

            return weightExercise;
        }

        public ExerciseEnvelope RemoveCardioExerciseFromExerciseEnvelope(ExerciseEnvelope exerciseEnvelope, CardioExercise cardioExercise)
        {
            var cardioExerciseToRemove = exerciseEnvelope.CardioExercises.Where(x => x.CardioExerciseId == cardioExercise.CardioExerciseId).FirstOrDefault();

            if (cardioExerciseToRemove is null)
                return null;

            exerciseEnvelope.CardioExercises.Remove(cardioExerciseToRemove);

            return exerciseEnvelope;
        }

        public ExerciseEnvelope RemoveWeightExerciseFromExerciseEnvelope(ExerciseEnvelope exerciseEnvelope, WeightExercise weightExercise)
        {
            var weightExerciseToRemove = exerciseEnvelope.WeightExercises.Where(x => x.WeightExerciseId == weightExercise.WeightExerciseId).FirstOrDefault();

            if (weightExerciseToRemove is null)
                return null;

            exerciseEnvelope.WeightExercises.Remove(weightExerciseToRemove);

            return exerciseEnvelope;
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

        public List<WeightExercise> ReturnWeightExercisesInExerciseEnvelope(ExerciseEnvelope exerciseEnvelope)
        {
            List<WeightExercise> weightExercisesToReturn = new List<WeightExercise>();

            if (exerciseEnvelope.WeightExercises is null)
                return null;

            foreach (var weights in exerciseEnvelope.WeightExercises)
            {
                weightExercisesToReturn.Add(weights);
            }

            return weightExercisesToReturn;
        }

        public ExerciseEnvelope UpdateCardioExerciseInExerciseEnvelope(ExerciseEnvelope exerciseToUpdate, CardioExercise cardioExercise)
        {
            var cardioExerciseToRemove = exerciseToUpdate.CardioExercises.Where(x => x.CardioExerciseId == cardioExercise.CardioExerciseId).FirstOrDefault();

            if (cardioExerciseToRemove is null)
                return null;

            exerciseToUpdate.CardioExercises.Remove(cardioExerciseToRemove);

            exerciseToUpdate.CardioExercises.Add(cardioExercise);

            return exerciseToUpdate;
        }

        public ExerciseEnvelope UpdateWeightExerciseInExerciseEnvelope(ExerciseEnvelope exerciseToUpdate, WeightExercise weightExercise)
        {
            var weightExerciseToRemove = exerciseToUpdate.WeightExercises.Where(x => x.WeightExerciseId == weightExercise.WeightExerciseId).FirstOrDefault();

            if (weightExerciseToRemove is null)
                return null;

            exerciseToUpdate.WeightExercises.Remove(weightExerciseToRemove);

            exerciseToUpdate.WeightExercises.Add(weightExercise);

            return exerciseToUpdate;
        }
    }
}
