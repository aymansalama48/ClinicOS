using ClinicOS.Domain.Common.Results;
using MediatR;


namespace ClinicOS.Application.Common.Abstractions.Messaging
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}
