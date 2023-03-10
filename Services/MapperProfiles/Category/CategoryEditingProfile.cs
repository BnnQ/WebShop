using AutoMapper;
using Homework.Models.Category;

namespace Homework.Services.MapperProfiles.Category
{
    public class CategoryEditingProfile : Profile
    {
        public CategoryEditingProfile() 
        {
            CreateMap<CategoryEditingDto, Data.Entities.Category>()
                .ForMember(dest => dest.ChildCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}