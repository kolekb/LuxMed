using AutoMapper;
using LuxMedTest.Application.Dtos;
using LuxMedTest.Domain.Models;

namespace LuxMedTest.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ExchangeRate, ExchangeRateDto>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Mid));
        }
    }
}
