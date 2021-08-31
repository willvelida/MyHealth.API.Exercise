using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Validators
{
    public interface IExerciseValidator
    {
        mdl.ExerciseEnvelope CreateValidExerciseEnvelope();
    }
}
