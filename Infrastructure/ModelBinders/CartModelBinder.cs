using Homework.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Homework.Infrastructure.ModelBinders;

public class CartModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext is null)
            throw new ArgumentNullException(nameof(bindingContext));
        
        var session = bindingContext.HttpContext.Session;
        if (session is null)
        {
            throw new InvalidOperationException("Session has not been enabled. Please call app.UseSession() in the Startup class.");
        }
        var cart = session.GetRequiredCart();
        
        bindingContext.Result = ModelBindingResult.Success(cart);
        return Task.CompletedTask;
    }
}