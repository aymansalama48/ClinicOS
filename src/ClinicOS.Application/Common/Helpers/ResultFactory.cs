namespace ClinicOS.Application.Common.Helpers;

using System;
using System.Linq;
using System.Reflection;
using ClinicOS.Domain.Common.Results;

public static class ResultFactory
{
    public static TResponse CreateFailure<TResponse>(Error error)
    {
        // 1. لو كان النوع Result العادي (بدون داتا)
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        // 2. لو كان النوع Result<T> (يحتوي على داتا)
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];

            var method = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
                .MakeGenericMethod(valueType);

            return (TResponse)method.Invoke(null, [error])!;
        }

        throw new InvalidOperationException("نوع الإرجاع غير مدعوم في الـ Pipeline.");
    }
}