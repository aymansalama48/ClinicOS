using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Features.Doctors.Queries.GetMyProfile;
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

namespace ClinicOS.UnitTests.Features.Doctors.Queries.GetMyProfile;

public class GetMyDoctorProfileQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly GetMyDoctorProfileQueryHandler _handler;

    public GetMyDoctorProfileQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _currentUserMock = new Mock<ICurrentUser>();
        _handler = new GetMyDoctorProfileQueryHandler(_contextMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Profile_Does_Not_Exist()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns(Guid.NewGuid());
        var doctors = new List<Doctor>().AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var query = new GetMyDoctorProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.ProfileNotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Doctor_Profile_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var specId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var doctor = Doctor.Create(userId, specId, "My Biography", 7, 220, 45);
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var query = new GetMyDoctorProfileQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(doctor.Id);
        result.Data.SpecializationId.Should().Be(specId);
        result.Data.Bio.Should().Be("My Biography");
        result.Data.YearsOfExperience.Should().Be(7);
        result.Data.ConsultationFee.Should().Be(220);
        result.Data.UrgentSurchargeFee.Should().Be(45);
        result.Data.IsActive.Should().BeTrue();
    }
}
