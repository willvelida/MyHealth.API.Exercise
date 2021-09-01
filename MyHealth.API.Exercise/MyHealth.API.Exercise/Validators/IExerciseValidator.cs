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
    }
}
