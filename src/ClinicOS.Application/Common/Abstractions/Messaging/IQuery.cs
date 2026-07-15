using ClinicOS.Domain.Common.Results;
using MediatR;

namespace ClinicOS.Application.Common.Abstractions.Messaging
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
