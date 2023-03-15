using AutoMapper;
using Homework.Models.Claim;

namespace Homework.Services.MapperProfiles.Claim;

public class ClaimInfoProfile : Profile
{
    public ClaimInfoProfile()
    {
        CreateMap<System.Security.Claims.Claim, ClaimInfoDto>().ReverseMap();
    }
}