using AutoMapper;
using Homework.Models.Claim;

namespace Homework.Services.MapperProfiles.Claim;

public class ClaimDeletingProfile : Profile
{
    public ClaimDeletingProfile()
    {
        CreateMap<System.Security.Claims.Claim, ClaimDeletingDto>().ReverseMap();
    }
}