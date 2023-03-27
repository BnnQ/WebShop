namespace Homework.Utils.Extensions;

public static class HttpContextExtensions
{
    public static string GetSiteBaseUrl(this HttpContext context)
    {
        var request = context.Request;
        return $"{request.Scheme}://{request.Host}{request.PathBase}";
    }
}