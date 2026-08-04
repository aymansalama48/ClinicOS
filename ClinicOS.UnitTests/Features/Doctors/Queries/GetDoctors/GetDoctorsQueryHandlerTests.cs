using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Doctors.Queries.GetDoctors;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Doctors.Queries.GetDoctors;

public class GetDoctorsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly GetDoctorsQueryHandler _handler;

    public GetDoctorsQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new GetDoctorsQueryHandler(_contextMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Paged_Doctors_With_User_Details()
    {
        // Arrange
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var specId = Guid.NewGuid();

        var doc1 = Doctor.Create(user1Id, specId, "Bio 1", 10, 200, 50);
        var doc2 = Doctor.Create(user2Id, specId, "Bio 2", 8, 180, 40);
        var doctors = new List<Doctor> { doc1, doc2 }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var usersList = new List<UserDto>
        {
            new() { Id = user1Id, FullName = "Dr. One", Email = "one@test.com" },
            new() { Id = user2Id, FullName = "Dr. Two", Email = "two@test.com" }
        };
        _userServiceMock.Setup(u => u.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(usersList);

        var query = new GetDoctorsQuery(null, null, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.Pagination.TotalCount.Should().Be(2);

        var firstItem = result.Data.Items.First();
        firstItem.FullName.Should().Be("Dr. One");
        firstItem.Email.Should().Be("one@test.com");
    }

    [Fact]
    public async Task Handle_Should_Filter_By_SpecializationId()
    {
        // Arrange
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();
        var targetSpec = Guid.NewGuid();
        var otherSpec = Guid.NewGuid();

        var doc1 = Doctor.Create(user1Id, targetSpec, "Bio 1", 10, 200, 50);
        var doc2 = Doctor.Create(user2Id, otherSpec, "Bio 2", 8, 180, 40);
        var doctors = new List<Doctor> { doc1, doc2 }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        _userServiceMock.Setup(u => u.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDto>
            {
                new() { Id = user1Id, FullName = "Dr. Target", Email = "target@test.com" }
            });

        var query = new GetDoctorsQuery(targetSpec, true, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().SpecializationId.Should().Be(targetSpec);
    }
}
