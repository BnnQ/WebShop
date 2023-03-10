using AutoMapper;
using Homework.Models.Product;

namespace Homework.Services.MapperProfiles.Product
{
    public class ProductEditingProfile : Profile
    {
        public ProductEditingProfile()
        {
            CreateMap<ProductEditingDto, Data.Entities.Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.AssociatedBanners, opt => opt.Ignore())
            .ReverseMap();
        }
    }
}