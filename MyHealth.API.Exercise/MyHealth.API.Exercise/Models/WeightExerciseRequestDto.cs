namespace MyHealth.API.Exercise.Models
{
    public class WeightExerciseRequestDto
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public string Notes { get; set; }
    }
}
