using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FleetErp.Api.Middleware;

/// <summary>
/// Ejecuta el validador de FluentValidation que corresponda a cada argumento del
/// controlador. Centralizarlo evita repetir tres líneas de validación en cada
/// acción y garantiza que ningún endpoint se olvide de validar.
/// </summary>
public sealed class FluentValidationFilter(IServiceProvider services) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values.Where(a => a is not null))
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            if (services.GetService(validatorType) is not IValidator validator) continue;

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument), context.HttpContext.RequestAborted);
            if (!result.IsValid) throw new ValidationException(result.Errors);
        }

        await next();
    }
}
