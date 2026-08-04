using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Features.Doctors.Commands.UpdateMyProfile;
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

namespace ClinicOS.UnitTests.Features.Doctors.Commands.UpdateMyProfile;

public class UpdateMyDoctorProfileCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly UpdateMyDoctorProfileCommandHandler _handler;

    public UpdateMyDoctorProfileCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _currentUserMock = new Mock<ICurrentUser>();
        _handler = new UpdateMyDoctorProfileCommandHandler(_contextMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Doctor_Profile_Missing()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns(Guid.NewGuid());
        var doctors = new List<Doctor>().AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var command = new UpdateMyDoctorProfileCommand(Guid.NewGuid(), "New Bio", 5, 200, 30);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.ProfileNotFound);

        _contextMock.Verify(c => c.Update(It.IsAny<Doctor>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_Doctor_Profile_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var doctor = Doctor.Create(userId, Guid.NewGuid(), "Old Bio", 3, 100, 20);
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var newSpecId = Guid.NewGuid();
        var command = new UpdateMyDoctorProfileCommand(newSpecId, "Updated Bio", 6, 250, 50);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(doctor.Id);

        doctor.SpecializationId.Should().Be(newSpecId);
        doctor.Bio.Should().Be("Updated Bio");
        doctor.YearsOfExperience.Should().Be(6);
        doctor.ConsultationFee.Should().Be(250);
        doctor.UrgentSurchargeFee.Should().Be(50);

        _contextMock.Verify(c => c.Update(doctor), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
