namespace Homework.ViewModels.Claim;

public class ListViewModel
{
    public IEnumerable<System.Security.Claims.Claim>? Claims { get; set; }
    public string UserId { get; set; } = null!;
}