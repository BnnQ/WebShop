using AutoMapper;
using Homework.Models.Role;
using Microsoft.AspNetCore.Identity;

namespace Homework.Services.MapperProfiles.Role
{
    public class RoleEditingProfile : Profile
    {
        public RoleEditingProfile() 
        {
            CreateMap<RoleEditingDto, IdentityRole>()
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()))
            .ReverseMap();
        }
    }
}