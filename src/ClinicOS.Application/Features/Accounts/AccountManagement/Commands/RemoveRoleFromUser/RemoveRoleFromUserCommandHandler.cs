using ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;
using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Domain.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Commands.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandHandler : ICommandHandler<RemoveRoleFromUserCommand, bool>
{
    private readonly IUserManagementService _userService;
    private readonly IJobScheduler _jobScheduler;

    public RemoveRoleFromUserCommandHandler(IUserManagementService userService, IJobScheduler jobScheduler)
    {
        _userService = userService;
        _jobScheduler = jobScheduler;
    }

    public async Task<Result<bool>> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.RemoveRoleAsync(request.UserId, request.RoleName, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.Errors);
        }

        var userResult = await _userService.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsSuccess && !string.IsNullOrWhiteSpace(userResult.Data.Email))
        {
            var templateModel = new RoleRemovedTemplateModel
            {
                RoleName = request.RoleName
            };
            var email = userResult.Data.Email;

            // إرسال الإيميل في الخلفية
            _jobScheduler.Enqueue<IIdentityNotificationService>(n =>
                n.SendRoleRemovedEmailAsync(email, templateModel));
        }

        return Result<bool>.Success(true);
    }
}