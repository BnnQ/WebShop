namespace Homework.Models.Claim;

public class ClaimDeletingDto : ClaimInfoDto
{
    public string Issuer { get; set; } = null!;
    public string OriginalIssuer { get; set; } = null!;

    public ClaimDeletingDto()
    {
        //empty
    }

    public ClaimDeletingDto(string userId, string type, string value, string valueType, string issuer, string originalIssuer) : base(userId, type, value, valueType)
    {
        Issuer = issuer;
        OriginalIssuer = originalIssuer;
    }
    
    /// <inheritdoc/>
    public ClaimDeletingDto(string claimInfoString, char separator = ';') : base(claimInfoString, separator)
    {
        //empty
    }
}