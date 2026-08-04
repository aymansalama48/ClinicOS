using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Features.Doctors.Queries.GetDoctorById;
using ClinicOS.Domain.Common.Results;
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

namespace ClinicOS.UnitTests.Features.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly GetDoctorByIdQueryHandler _handler;

    public GetDoctorByIdQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new GetDoctorByIdQueryHandler(_contextMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Doctor_Does_Not_Exist()
    {
        // Arrange
        var doctors = new List<Doctor>().AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var query = new GetDoctorByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.NotFound);

        _userServiceMock.Verify(u => u.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Doctor_Details_With_UserData_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var specId = Guid.NewGuid();
        var doctor = Doctor.Create(userId, specId, "Consultant Bio", 15, 300, 50);
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var userDto = new UserDto
        {
            Id = userId,
            FullName = "Dr. Ahmed Ali",
            Email = "ahmed@example.com",
            AvatarUrl = "http://example.com/avatar.png"
        };
        _userServiceMock.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<UserDto>.Success(userDto));

        var query = new GetDoctorByIdQuery(doctor.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(doctor.Id);
        result.Data.FullName.Should().Be("Dr. Ahmed Ali");
        result.Data.Email.Should().Be("ahmed@example.com");
        result.Data.SpecializationId.Should().Be(specId);
        result.Data.YearsOfExperience.Should().Be(15);
    }
}
