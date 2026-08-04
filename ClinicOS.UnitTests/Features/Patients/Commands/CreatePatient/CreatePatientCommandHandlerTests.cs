using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Patients;
using ClinicOS.Application.Features.Patients.Commands.CreatePatient;
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

namespace ClinicOS.UnitTests.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CreatePatientCommandHandler _handler;

    public CreatePatientCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _handler = new CreatePatientCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_PhoneNumber_Already_Exists()
    {
        // Arrange
        var existingPatient = new Patient
        {
            FirstName = "Ali",
            LastName = "Hassan",
            PhoneNumber = "01012345678"
        };
        var patients = new List<Patient> { existingPatient }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new CreatePatientCommand(
            "Omar",
            null,
            "Khaled",
            "01012345678",
            new DateOnly(1995, 5, 20),
            Gender.Male,
            BloodType.APositive,
            "01111111111",
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(PatientErrors.PhoneNumberAlreadyExists);

        _contextMock.Verify(c => c.Add(It.IsAny<Patient>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Create_Patient_When_Valid()
    {
        // Arrange
        var patients = new List<Patient>().AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var command = new CreatePatientCommand(
            "Youssef",
            "Mohamed",
            "Ibrahim",
            "01099998888",
            new DateOnly(2000, 1, 1),
            Gender.Male,
            BloodType.OPositive,
            "01222222222",
            null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _contextMock.Verify(c => c.Add(It.Is<Patient>(p =>
            p.FirstName == "Youssef" &&
            p.PhoneNumber == "01099998888")), Times.Once);

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
