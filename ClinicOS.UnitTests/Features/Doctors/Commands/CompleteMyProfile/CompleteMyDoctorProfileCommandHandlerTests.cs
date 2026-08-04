using ClinicOS.Application.Common.Abstractions.Identity.CurrentUser;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Common.Errors.Users;
using ClinicOS.Application.Features.Doctors.Commands.CompleteMyProfile;
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

namespace ClinicOS.UnitTests.Features.Doctors.Commands.CompleteMyProfile;

public class CompleteMyDoctorProfileCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly CompleteMyDoctorProfileCommandHandler _handler;

    public CompleteMyDoctorProfileCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _currentUserMock = new Mock<ICurrentUser>();
        _handler = new CompleteMyDoctorProfileCommandHandler(_contextMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_User_Id_Is_Missing()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns((Guid?)null);
        var command = new CompleteMyDoctorProfileCommand(Guid.NewGuid(), "Bio", 10, 200, 50);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Profile_Already_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var existingDoctor = Doctor.Create(userId, Guid.NewGuid(), "Bio", 5, 100, 20);
        var doctors = new List<Doctor> { existingDoctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var command = new CompleteMyDoctorProfileCommand(Guid.NewGuid(), "Bio", 10, 200, 50);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.ProfileAlreadyExists);
    }

    [Fact]
    public async Task Handle_Should_Create_Doctor_Profile_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var specId = Guid.NewGuid();
        _currentUserMock.Setup(u => u.UserId).Returns(userId);

        var doctors = new List<Doctor>().AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var command = new CompleteMyDoctorProfileCommand(specId, "Specialist Surgeon", 12, 350, 75);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _contextMock.Verify(c => c.Add(It.Is<Doctor>(d =>
            d.ApplicationUserId == userId &&
            d.SpecializationId == specId &&
            d.ConsultationFee == 350 &&
            d.UrgentSurchargeFee == 75)), Times.Once);

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
