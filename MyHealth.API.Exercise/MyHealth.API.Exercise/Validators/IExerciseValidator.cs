using System;
using System.Collections.Generic;
using System.Text;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Validators
{
    public interface IExerciseValidator
    {
        mdl.ExerciseEnvelope CreateValidExerciseEnvelope();
    }
}
