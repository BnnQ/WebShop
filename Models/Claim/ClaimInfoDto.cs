using System.Security.Claims;
namespace Homework.Models.Claim;

public class ClaimInfoDto
{
    public string UserId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string ValueType { get; set; } = null!;

    public ClaimInfoDto()
    {
        //empty
    }
    
    public ClaimInfoDto(string userId, string type, string value, string valueType)
    {
        UserId = userId;
        Type = type;
        Value = value;
        ValueType = valueType;
    }
    
    /// <summary>
    /// <para>Accepts a string containing <see cref="Claim"/> values. There must be 4 values in this order: UserId;Type;Value;ValueType</para>
    /// <para>Where instead of ';' can be any character specified in the <paramref name="separator"></paramref> argument.</para>
    /// <para>If the format does not match, it throws an <see cref="ArgumentException"/>.</para>
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public ClaimInfoDto(string claimInfoString, char separator = ';')
    {
        try
        {
            var values = claimInfoString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            UserId = values.First();
            Type = values[1];
            Value = values[2];
            ValueType = values.Last();
        }
        catch
        {
            throw new ArgumentException("Value doesn't match specified format. See docs.", nameof(claimInfoString));
        }
    }

    public override string ToString()
    {
        return $"{UserId};{Type};{Value};{ValueType}";
    }
}