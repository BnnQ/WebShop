using AutoMapper;
using Homework.Models.Manufacturer;

namespace Homework.Services.MapperProfiles.Manufacturer
{
    public class ManufacturerCreationProfile : Profile
    {
        public ManufacturerCreationProfile() 
        {
            CreateMap<ManufacturerCreationDto, Data.Entities.Manufacturer>();
        }
    }
}