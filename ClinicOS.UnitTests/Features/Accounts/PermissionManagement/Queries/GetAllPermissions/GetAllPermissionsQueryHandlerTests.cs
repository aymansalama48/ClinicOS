using ClinicOS.Application.Common.Abstractions.Identity.Authorization;
using ClinicOS.Application.Common.Abstractions.Identity.Authorization.Models;
using ClinicOS.Application.Features.Accounts.PermissionManagement.Queries.GetAllPermissions;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.PermissionManagement.Queries.GetAllPermissions;

public class GetAllPermissionsQueryHandlerTests
{
    private readonly Mock<IPermissionManagementService> _serviceMock;
    private readonly GetAllPermissionsQueryHandler _handler;

    public GetAllPermissionsQueryHandlerTests()
    {
        _serviceMock = new Mock<IPermissionManagementService>();
        _handler = new GetAllPermissionsQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Permissions()
    {
        // Arrange
        var list = new List<PermissionDto>
        {
            new(Guid.NewGuid(), "Doctors.View", "View Doctors", "Doctors", "Can view doctors"),
            new(Guid.NewGuid(), "Doctors.Create", "Create Doctors", "Doctors", "Can create doctors")
        };

        _serviceMock.Setup(s => s.GetAllPermissionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<PermissionDto>>.Success(list));

        var query = new GetAllPermissionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }
}
