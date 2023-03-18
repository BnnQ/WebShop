namespace Homework.ViewModels.Cart;

public class ListViewModel
{
    public Models.Domain.Cart Cart { get; set; } = null!;
    public string? ReturnUrl { get; set; }
}