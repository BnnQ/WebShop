using Homework.Infrastructure.ModelBinders;
using Homework.Models.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Homework.Infrastructure.ModelBinderProviders;

public class CartModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(Cart) ? new CartModelBinder() : null;
    }
}