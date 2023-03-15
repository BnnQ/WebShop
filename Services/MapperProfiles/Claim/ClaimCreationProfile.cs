using AutoMapper;
using Homework.Models.Claim;

namespace Homework.Services.MapperProfiles.Claim;

public class ClaimCreationProfile : Profile
{
    public ClaimCreationProfile()
    {
        CreateMap<System.Security.Claims.Claim, ClaimCreationDto>().ReverseMap();
    }
}