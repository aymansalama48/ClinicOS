using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Patients;
using ClinicOS.Application.Features.Patients.Commands.UpdatePatient;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Enums;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly UpdatePatientCommandHandler _handler;

    public UpdatePatientCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _handler = new UpdatePatientCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Patient_Does_Not_Exist()
    {
        // Arrange
        var patients = new List<Patient>().AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new UpdatePatientCommand(
            Guid.NewGuid(),
            "Ahmed",
            null,
            "Ali",
            "01012345678",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            BloodType.BPositive,
            null,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(PatientErrors.NotFound);

        _contextMock.Verify(c => c.Update(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Phone_Belongs_To_Another_Patient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var currentPatient = new Patient { Id = patientId, FirstName = "Ahmed", LastName = "Ali", PhoneNumber = "01011111111" };
        var otherPatient = new Patient { Id = Guid.NewGuid(), FirstName = "Mohamed", LastName = "Sayed", PhoneNumber = "01022222222" };
        var patients = new List<Patient> { currentPatient, otherPatient }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new UpdatePatientCommand(
            patientId,
            "Ahmed",
            null,
            "Ali",
            "01022222222", // same as otherPatient
            new DateOnly(1990, 1, 1),
            Gender.Male,
            BloodType.BPositive,
            null,
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(PatientErrors.PhoneNumberAlreadyExists);

        _contextMock.Verify(c => c.Update(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_Patient_When_Valid()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var currentPatient = new Patient { Id = patientId, FirstName = "Ahmed", LastName = "Ali", PhoneNumber = "01011111111" };
        var patients = new List<Patient> { currentPatient }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new UpdatePatientCommand(
            patientId,
            "Ahmed Updated",
            "Mahmoud",
            "Ali",
            "01099999999",
            new DateOnly(1992, 3, 15),
            Gender.Male,
            BloodType.ONegative,
            "01200000000",
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(patientId);

        currentPatient.FirstName.Should().Be("Ahmed Updated");
        currentPatient.MiddleName.Should().Be("Mahmoud");
        currentPatient.PhoneNumber.Should().Be("01099999999");

        _contextMock.Verify(c => c.Update(currentPatient), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
