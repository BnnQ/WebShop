using AutoMapper;
using Homework.ViewModels.Account;

namespace Homework.Services.MapperProfiles.User
{
    public class RegistrationUserProfile : Profile
    {
        public RegistrationUserProfile()
        {
            CreateMap<RegistrationViewModel, Data.Entities.User>();
        }
    }
}