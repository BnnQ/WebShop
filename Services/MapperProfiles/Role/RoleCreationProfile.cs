using AutoMapper;
using Homework.Models.Role;
using Microsoft.AspNetCore.Identity;

namespace Homework.Services.MapperProfiles.Role
{
    public class RoleCreationProfile : Profile
    {
        public RoleCreationProfile() 
        {
            CreateMap<RoleCreationDto, IdentityRole>()
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()));
        }    
    }
}