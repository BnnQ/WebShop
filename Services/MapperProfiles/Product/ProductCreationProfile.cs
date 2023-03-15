using AutoMapper;
using Homework.Models.Product;

namespace Homework.Services.MapperProfiles.Product
{
    public class ProductCreationProfile : Profile
    {
        public ProductCreationProfile()
        {
            CreateMap<ProductCreationDto, Data.Entities.Product>().ReverseMap();
        }
    }
}