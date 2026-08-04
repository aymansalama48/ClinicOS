using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetRolePermissions;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PermissionManagement.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandlerTests
{
    private readonly Mock<IPermissionManagementService> _serviceMock;
    private readonly GetRolePermissionsQueryHandler _handler;

    public GetRolePermissionsQueryHandlerTests()
    {
        _serviceMock = new Mock<IPermissionManagementService>();
        _handler = new GetRolePermissionsQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Role_Permissions()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var dto = new RoleWithPermissionsDto(roleId, "Doctor", "Doctor Role", false, new List<PermissionDto>
        {
            new(Guid.NewGuid(), "Doctors.View", "View Doctors", "Doctors", "Can view doctors")
        });

        _serviceMock.Setup(s => s.GetRolePermissionsAsync(roleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RoleWithPermissionsDto>.Success(dto));

        var query = new GetRolePermissionsQuery(roleId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(dto);
    }
}
