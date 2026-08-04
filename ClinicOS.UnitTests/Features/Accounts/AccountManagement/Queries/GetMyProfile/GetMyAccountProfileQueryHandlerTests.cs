using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetMyProfile;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Queries.GetMyProfile;

public class GetMyAccountProfileQueryHandlerTests
{
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly GetMyAccountProfileQueryHandler _handler;

    public GetMyAccountProfileQueryHandlerTests()
    {
        _currentUserMock = new Mock<ICurrentUser>();
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new GetMyAccountProfileQueryHandler(_currentUserMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_CurrentUser_Is_Invalid()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns((Guid?)null);

        var query = new GetMyAccountProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.NotFound);

        _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_UserService_Returns_Failure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        _userServiceMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Failure(UserErrors.NotFound));

        var query = new GetMyAccountProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Profile_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var userDto = new UserDto
        {
            Id = userId,
            FirstName = "Ayman",
            MiddleName = "Mohamed",
            LastName = "Salama",
            FullName = "Ayman Mohamed Salama",
            Email = "ayman@example.com",
            PhoneNumber = "01012345678",
            AvatarUrl = "https://example.com/avatar.jpg"
        };

        _userServiceMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(userDto));

        var query = new GetMyAccountProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FirstName.Should().Be("Ayman");
        result.Data.FullName.Should().Be("Ayman Mohamed Salama");
        result.Data.Email.Should().Be("ayman@example.com");
    }
}
