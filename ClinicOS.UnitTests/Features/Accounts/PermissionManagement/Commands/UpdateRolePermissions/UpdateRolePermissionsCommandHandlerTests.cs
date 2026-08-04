using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.UpdateRolePermissions;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PermissionManagement.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsCommandHandlerTests
{
    private readonly Mock<IPermissionManagementService> _serviceMock;
    private readonly UpdateRolePermissionsCommandHandler _handler;

    public UpdateRolePermissionsCommandHandlerTests()
    {
        _serviceMock = new Mock<IPermissionManagementService>();
        _handler = new UpdateRolePermissionsCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Updated()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        _serviceMock.Setup(s => s.UpdateRolePermissionsAsync(roleId, permissionIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var command = new UpdateRolePermissionsCommand(roleId, permissionIds);

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
        var permissionIds = new List<Guid> { Guid.NewGuid() };
        var error = new Error("Role.NotFound", "Role not found", ErrorType.NotFound);

        _serviceMock.Setup(s => s.UpdateRolePermissionsAsync(roleId, permissionIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new UpdateRolePermissionsCommand(roleId, permissionIds);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
