using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Messaging;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Domain.Common.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetMyProfile;

public sealed class GetMyAccountProfileQueryHandler : IQueryHandler<GetMyAccountProfileQuery, MyAccountProfileResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserManagementService _userService;

    public GetMyAccountProfileQueryHandler(ICurrentUser currentUser, IUserManagementService userService)
    {
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<Result<MyAccountProfileResponse>> Handle(GetMyAccountProfileQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue || _currentUser.UserId.Value == Guid.Empty)
        {
            return Result<MyAccountProfileResponse>.Failure(UserErrors.NotFound);
        }

        var userResult = await _userService.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Result<MyAccountProfileResponse>.Failure(UserErrors.NotFound);
        }

        var userDetails = userResult.Data;

        return Result<MyAccountProfileResponse>.Success(new MyAccountProfileResponse(
            userDetails.FirstName,
            userDetails.MiddleName,
            userDetails.LastName,
            userDetails.FullName,
            userDetails.Email,
            userDetails.PhoneNumber,
            userDetails.AvatarUrl
        ));
    }
}