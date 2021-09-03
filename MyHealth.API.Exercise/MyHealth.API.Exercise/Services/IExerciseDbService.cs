using System.Collections.Generic;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Services
{
    public interface IExerciseDbService
    {
        Task CreateWorkout(mdl.ExerciseEnvelope exerciseEnvelope);
        Task<mdl.ExerciseEnvelope> GetWorkoutByDate(string date);
        Task<mdl.ExerciseEnvelope> GetWorkoutById(string id);
        Task<List<mdl.ExerciseEnvelope>> GetAllWorkouts();
        Task UpdateWorkout(mdl.ExerciseEnvelope exerciseEnvelope);
        Task DeleteWorkout(mdl.ExerciseEnvelope exerciseEnvelope);
        Task CreateWeightExercise(mdl.ExerciseEnvelope workout, mdl.WeightExercise weightExercise);
        Task CreateCardioExercise(mdl.ExerciseEnvelope workout, mdl.CardioExercise cardioExercise);
    }
}
