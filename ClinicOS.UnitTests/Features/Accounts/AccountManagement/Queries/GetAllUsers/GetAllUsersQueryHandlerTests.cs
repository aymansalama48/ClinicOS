using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Accounts.AccountManagement.Queries.GetAllUsers;
using ClinicOS.Domain.Common.Results;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.AccountManagement.Queries.GetAllUsers;

public class GetAllUsersQueryHandlerTests
{
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new GetAllUsersQueryHandler(_userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Paged_Users()
    {
        // Arrange
        var usersList = new List<UserDto>
        {
            new() { Id = Guid.NewGuid(), FullName = "User 1", Email = "u1@test.com", Roles = new List<string> { "Doctor" } },
            new() { Id = Guid.NewGuid(), FullName = "User 2", Email = "u2@test.com", Roles = new List<string> { "Receptionist" } }
        };

        var pagedResult = new PagedResult<UserDto>
        {
            Items = usersList,
            Pagination = new PaginationMetadata
            {
                CurrentPage = 1,
                PageSize = 10,
                TotalCount = 2
            }
        };

        _userServiceMock.Setup(u => u.GetAllUsersAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var query = new GetAllUsersQuery(1, 10, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.Pagination.TotalCount.Should().Be(2);
        result.Data.Pagination.CurrentPage.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Pass_Filters_To_Service()
    {
        // Arrange
        var pagedResult = new PagedResult<UserDto>
        {
            Items = new List<UserDto>(),
            Pagination = new PaginationMetadata { CurrentPage = 2, PageSize = 5, TotalCount = 0 }
        };

        _userServiceMock.Setup(u => u.GetAllUsersAsync(2, 5, "Admin", "Ahmed", It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var query = new GetAllUsersQuery(2, 5, "Admin", "Ahmed");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Pagination.CurrentPage.Should().Be(2);
        result.Data.Pagination.PageSize.Should().Be(5);
    }
}
