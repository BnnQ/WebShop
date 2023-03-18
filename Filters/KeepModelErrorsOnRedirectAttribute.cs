using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Homework.Filters;
public class KeepModelErrorsOnRedirectAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        base.OnResultExecuting(context);
        if (context.Result is not (RedirectResult or RedirectToActionResult)) return;
        if (context.Controller is not Controller controller) return;
        
        var modelState = controller.ViewData.ModelState;
        if (!modelState.IsValid)
        {
            var errorList = modelState.Where(item => item.Value?.Errors.Any() is true).ToDictionary(item => item.Key,
                item => item.Value!.Errors.Select(error => error.ErrorMessage)
                    .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message)));

            controller.TempData["ModelErrorList"] = JsonSerializer.Serialize(errorList);
        }
    }
}