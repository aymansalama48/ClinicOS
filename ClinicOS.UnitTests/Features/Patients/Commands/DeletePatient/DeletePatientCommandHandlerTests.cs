using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Patients;
using ClinicOS.Application.Features.Patients.Commands.DeletePatient;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly DeletePatientCommandHandler _handler;

    public DeletePatientCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new DeletePatientCommandHandler(_contextMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Patient_Does_Not_Exist()
    {
        // Arrange
        var patients = new List<Patient>().AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new DeletePatientCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(PatientErrors.NotFound);

        _contextMock.Verify(c => c.Remove(It.IsAny<Patient>()), Times.Never);
        _userServiceMock.Verify(u => u.DeactivateUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Delete_Without_Deactivating_User_When_Account_Not_Linked()
    {
        // Arrange
        var patient = new Patient { Id = Guid.NewGuid(), ApplicationUserId = null };
        var patients = new List<Patient> { patient }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new DeletePatientCommand(patient.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _contextMock.Verify(c => c.Remove(patient), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userServiceMock.Verify(u => u.DeactivateUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Delete_And_Deactivate_User_When_Account_Is_Linked()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var patient = new Patient { Id = Guid.NewGuid(), ApplicationUserId = userId };
        var patients = new List<Patient> { patient }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new DeletePatientCommand(patient.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _contextMock.Verify(c => c.Remove(patient), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userServiceMock.Verify(u => u.DeactivateUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
