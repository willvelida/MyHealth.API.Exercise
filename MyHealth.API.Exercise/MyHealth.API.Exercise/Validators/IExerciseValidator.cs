using System.Collections.Generic;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Validators
{
    public interface IExerciseValidator
    {
        mdl.ExerciseEnvelope CreateValidExerciseEnvelope();
        mdl.CardioExercise CreateValidCardioExerciseObject(mdl.CardioExercise cardioExercise);
        mdl.WeightExercise CreateValidWeightExerciseObject(mdl.WeightExercise weightExercise);
        List<mdl.CardioExercise> ReturnCardioExercisesInExerciseEnvelope(mdl.ExerciseEnvelope exerciseEnvelope);
        List<mdl.WeightExercise> ReturnWeightExercisesInExerciseEnvelope(mdl.ExerciseEnvelope exerciseEnvelope);
        mdl.WeightExercise GetWeightExerciseById(List<mdl.WeightExercise> weightExercises, string weightExerciseId);
        mdl.WeightExercise GetWeightExerciseById(mdl.ExerciseEnvelope exercise, string weightExerciseId);
        mdl.CardioExercise GetCardioExerciseById(List<mdl.CardioExercise> cardioExercises, string cardioExerciseId);
        mdl.CardioExercise GetCardioExerciseById(mdl.ExerciseEnvelope exercise, string cardioExerciseId);
        mdl.ExerciseEnvelope UpdateCardioExerciseInExerciseEnvelope(mdl.ExerciseEnvelope exerciseToUpdate, mdl.CardioExercise cardioExercise);
        mdl.ExerciseEnvelope UpdateWeightExerciseInExerciseEnvelope(mdl.ExerciseEnvelope exerciseToUpdate, mdl.WeightExercise weightExercise);
        mdl.ExerciseEnvelope RemoveCardioExerciseFromExerciseEnvelope(mdl.ExerciseEnvelope exerciseEnvelope, mdl.CardioExercise cardioExercise);
    }
}
