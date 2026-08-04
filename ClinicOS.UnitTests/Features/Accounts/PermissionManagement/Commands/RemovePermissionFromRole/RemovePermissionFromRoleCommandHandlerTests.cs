using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.RemovePermissionFromRole;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PermissionManagement.Commands.RemovePermissionFromRole;

public class RemovePermissionFromRoleCommandHandlerTests
{
    private readonly Mock<IPermissionManagementService> _serviceMock;
    private readonly RemovePermissionFromRoleCommandHandler _handler;

    public RemovePermissionFromRoleCommandHandlerTests()
    {
        _serviceMock = new Mock<IPermissionManagementService>();
        _handler = new RemovePermissionFromRoleCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Removed()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        _serviceMock.Setup(s => s.RemovePermissionFromRoleAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var command = new RemovePermissionFromRoleCommand(roleId, permissionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Service_Fails()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var error = new Error("Permission.NotFound", "Permission not assigned", ErrorType.NotFound);

        _serviceMock.Setup(s => s.RemovePermissionFromRoleAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new RemovePermissionFromRoleCommand(roleId, permissionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
