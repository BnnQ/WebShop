using AutoMapper;
using Homework.Models.Claim;

namespace Homework.Services.MapperProfiles.Claim;

public class ClaimInfoCreationProfile : Profile
{
    public ClaimInfoCreationProfile()
    {
        CreateMap<ClaimInfoDto, ClaimCreationDto>().ReverseMap();
    }
}