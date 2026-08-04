using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Features.Doctors.Commands.DeleteDoctor;
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

namespace ClinicOS.UnitTests.Features.Doctors.Commands.DeleteDoctor;

public class DeleteDoctorCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly DeleteDoctorCommandHandler _handler;

    public DeleteDoctorCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new DeleteDoctorCommandHandler(_contextMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Doctor_Not_Found()
    {
        // Arrange
        var doctors = new List<Doctor>().AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var command = new DeleteDoctorCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.NotFound);

        _contextMock.Verify(c => c.Remove(It.IsAny<Doctor>()), Times.Never);
        _userServiceMock.Verify(u => u.DeactivateUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Delete_Doctor_And_DeactivateUser_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var doctor = Doctor.Create(userId, Guid.NewGuid(), "Bio", 5, 200, 50);
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var command = new DeleteDoctorCommand(doctor.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _contextMock.Verify(c => c.Remove(doctor), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userServiceMock.Verify(u => u.DeactivateUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
