using ClinicOS.Domain.Common.Results;
using FluentValidation;
using MediatR;

namespace ClinicOS.Application.Common.Behaviors;

/// <summary>
/// سلوك التحقق من صحة مدخلات الطلب (Validation Behavior).
/// يجمع أخطاء FluentValidation ويرجعها كـ Result.Failure بدون Exception.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorMessages = failures.Select(f => f.ErrorMessage).ToList();

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var method = typeof(TResponse).GetMethod("Failure", new[] { typeof(IEnumerable<string>) });
                return (TResponse)method?.Invoke(null, new object[] { errorMessages })!;
            }

            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(errorMessages);
            }
        }

        return await next();
    }
}