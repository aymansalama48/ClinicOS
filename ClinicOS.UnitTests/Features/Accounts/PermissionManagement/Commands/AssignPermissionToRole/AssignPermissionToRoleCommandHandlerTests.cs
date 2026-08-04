using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Commands.AssignPermissionToRole;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PermissionManagement.Commands.AssignPermissionToRole;

public class AssignPermissionToRoleCommandHandlerTests
{
    private readonly Mock<IPermissionManagementService> _serviceMock;
    private readonly AssignPermissionToRoleCommandHandler _handler;

    public AssignPermissionToRoleCommandHandlerTests()
    {
        _serviceMock = new Mock<IPermissionManagementService>();
        _handler = new AssignPermissionToRoleCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Assigned()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        _serviceMock.Setup(s => s.AssignPermissionToRoleAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var command = new AssignPermissionToRoleCommand(roleId, permissionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Role_Or_Permission_Invalid()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();
        var error = new Error("Role.NotFound", "Role not found", ErrorType.NotFound);

        _serviceMock.Setup(s => s.AssignPermissionToRoleAsync(roleId, permissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(error));

        var command = new AssignPermissionToRoleCommand(roleId, permissionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error);
    }
}
