namespace Homework.ViewModels;

public class OrderConfirmationViewModel
{
    public Models.Domain.Cart Cart { get; set; }
    public string ConfirmationCode { get; set; }
    public  string SiteBaseUrl { get; set; }

    public OrderConfirmationViewModel(Models.Domain.Cart cart, string confirmationCode, string siteBaseUrl)
    {
        Cart = cart;
        ConfirmationCode = confirmationCode;
        SiteBaseUrl = siteBaseUrl;
    }
}