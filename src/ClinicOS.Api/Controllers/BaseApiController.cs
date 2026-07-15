using ClinicOS.Api.Extensions;
using ClinicOS.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// حقن تلقائي لـ MediatR Sender من حاوية DI
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// تحويل الـ Result إلى HTTP Response قياسي بنفس الـ HttpContext
    /// </summary>
    protected IResult HandleResult(Result result) => result.ToHttpResponse(HttpContext);

    protected IResult HandleResult<T>(Result<T> result) => result.ToHttpResponse(HttpContext);
}