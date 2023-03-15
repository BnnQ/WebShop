namespace Homework.Models.Claim;

public class ClaimCreationDto
{
    public string UserId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
}