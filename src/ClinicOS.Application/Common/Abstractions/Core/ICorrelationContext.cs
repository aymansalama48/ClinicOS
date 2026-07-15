namespace ClinicOS.Application.Common.Abstractions.Core;

public interface ICorrelationContext
{
    string CorrelationId { get; }
}