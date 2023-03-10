using AutoMapper;
using Homework.Models.User;

namespace Homework.Services.MapperProfiles.User
{
    public class UserEditingProfile : Profile
    {
        public UserEditingProfile()
        {
            CreateMap<UserEditingDto, Data.Entities.User>().ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore()).ReverseMap();
        } 
    }
}