using Homework.Models.Claim;

namespace Homework.Utils.Comparers;

public class ClaimInfoDtoEqualityComparer : IEqualityComparer<ClaimInfoDto>
{
    public bool Equals(ClaimInfoDto? x, ClaimInfoDto? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        return x.Type == y.Type && x.Value == y.Value && x.ValueType == y.ValueType;
    }

    public int GetHashCode(ClaimInfoDto obj)
    {
        return HashCode.Combine(obj.Type, obj.Value, obj.ValueType);
    }
}