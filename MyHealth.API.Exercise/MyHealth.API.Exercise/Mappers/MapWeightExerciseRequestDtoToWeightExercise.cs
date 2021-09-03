using AutoMapper;
using MyHealth.API.Exercise.Models;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Mappers
{
    public class MapWeightExerciseRequestDtoToWeightExercise : Profile
    {
        public MapWeightExerciseRequestDtoToWeightExercise()
        {
            CreateMap<WeightExerciseRequestDto, mdl.WeightExercise>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Reps, opt => opt.MapFrom(src => src.Reps))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
        }
    }
}
