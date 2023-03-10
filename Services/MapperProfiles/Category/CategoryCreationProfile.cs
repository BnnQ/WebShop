using AutoMapper;
using Homework.Models.Category;

namespace Homework.Services.MapperProfiles.Category
{
    public class CategoryCreationProfile : Profile
    {
        public CategoryCreationProfile() 
        {
            CreateMap<CategoryCreationDto, Data.Entities.Category>();
        }
    }
}