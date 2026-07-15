using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Domain.Common.Results;
using MediatR;

namespace ClinicOS.Application.Common.Behaviors;

/// <summary>
/// سلوك التحقق من الصلاحيات (Authorization Behavior).
/// يفحص وجود [PermissionAttribute] للطلب ويتحقق من هوية وصلاحيات المستخدم الحالي.
/// </summary>
public sealed class AuthorizationBehavior<TRequest, TResponse>(
    ICurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 1. استخراج الـ Attribute الخاص بالصلاحية من الـ Request
        var permissionAttribute = request.GetType()
            .GetCustomAttributes(typeof(PermissionAttribute), true)
            .FirstOrDefault() as PermissionAttribute;

        // إذا كان الطلب لا يتطلب صلاحية معينة، يمر مباشرة للـ Handler
        if (permissionAttribute is null)
            return await next();

        // 2. التحقق من حالة تسجيل الدخول
        if (!currentUser.IsAuthenticated)
        {
            return CreateFailureResponse("You are not authenticated.", ErrorType.Unauthorized);
        }

        // 3. التحقق من وجود معرف المستخدم
        var userId = currentUser.UserId.ToString();
        if (string.IsNullOrEmpty(userId))
        {
            return CreateFailureResponse("User identifier is missing.", ErrorType.Unauthorized);
        }

        // 4. التحقق من امتلاك المستخدم للصلاحية عبر ICurrentUser
        var hasPermission = currentUser.HasPermission(permissionAttribute.Name);

        if (!hasPermission)
        {
            return CreateFailureResponse(
                "You don't have permission to perform this action.",
                ErrorType.Forbidden);
        }

        return await next();
    }

    /// <summary>
    /// دالة إنشاء استجابة الفشل لضمان التوافق مع Result و Result<T>
    /// </summary>
    private static TResponse CreateFailureResponse(string message, ErrorType errorType)
    {
        var error = new Error($"AUTH_{errorType}", message, errorType);

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = typeof(TResponse).GetMethod("Failure", new[] { typeof(Error) });
            return (TResponse)failureMethod?.Invoke(null, new object[] { error })!;
        }

        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        throw new UnauthorizedAccessException(message);
    }
}