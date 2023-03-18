using Microsoft.AspNetCore.Mvc;

namespace Homework.Utils.Extensions;

public static class UrlHelperExtensions
{
    public static string ReturnUrl(this IUrlHelper urlHelper, HttpRequest httpRequest)
    {
        var returnUrl = "/";
        if (httpRequest.Path.HasValue && !httpRequest.Path.ToString().Contains("Account"))
        {
            returnUrl = httpRequest.Path + httpRequest.QueryString;
        }

        return returnUrl;
    }
}