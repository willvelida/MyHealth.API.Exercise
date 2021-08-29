using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Services
{
    public interface IExerciseDbService
    {
        Task CreateWorkout(mdl.ExerciseEnvelope exerciseEnvelope);
        Task<mdl.ExerciseEnvelope> GetWorkoutByDate(string date);
        Task<List<mdl.ExerciseEnvelope>> GetAllWorkouts();
        Task UpdateWorkout(mdl.ExerciseEnvelope exerciseEnvelope);
        Task DeleteWorkout(mdl.ExerciseEnvelope exerciseEnvelope);
        Task CreateWeightExercise();
        Task ReadWeightExercise();
        Task UpdateWeightExercise();
        Task DeleteWeightExercise();
        Task CreateCardioExercise();
        Task ReadCardioExercise();
        Task UpdateCardioExercise();
        Task DeleteCardioExercise();
    }
}
