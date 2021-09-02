using AutoMapper;
using MyHealth.API.Exercise.Models;
using mdl = MyHealth.Common.Models;

namespace MyHealth.API.Exercise.Mappers
{
    public class MapCardioExerciseRequestDtoToCardioExercise : Profile
    {
        public MapCardioExerciseRequestDtoToCardioExercise()
        {
            CreateMap<CardioExerciseRequestDto, mdl.CardioExercise>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DurationInMinutes, opt => opt.MapFrom(src => src.DurationInMinutes));
        }
    }
}
