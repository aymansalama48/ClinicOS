using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllRolesWithPermissions;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PermissionManagement.Queries.GetAllRolesWithPermissions;

public class GetAllRolesWithPermissionsQueryHandlerTests
{
    private readonly Mock<IPermissionManagementService> _serviceMock;
    private readonly GetAllRolesWithPermissionsQueryHandler _handler;

    public GetAllRolesWithPermissionsQueryHandlerTests()
    {
        _serviceMock = new Mock<IPermissionManagementService>();
        _handler = new GetAllRolesWithPermissionsQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Roles_With_Permissions()
    {
        // Arrange
        var list = new List<RoleWithPermissionsDto>
        {
            new(Guid.NewGuid(), "Admin", "System Admin", true, new List<PermissionDto>
            {
                new(Guid.NewGuid(), "Doctors.View", "View Doctors", "Doctors", "Can view doctors")
            })
        };

        _serviceMock.Setup(s => s.GetAllRolesWithPermissionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<RoleWithPermissionsDto>>.Success(list));

        var query = new GetAllRolesWithPermissionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().RoleName.Should().Be("Admin");
    }
}
