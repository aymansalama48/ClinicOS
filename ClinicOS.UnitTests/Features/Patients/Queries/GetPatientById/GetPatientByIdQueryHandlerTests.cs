using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Patients;
using ClinicOS.Application.Features.Patients.Queries.GetPatientById;
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

namespace ClinicOS.UnitTests.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetPatientByIdQueryHandler _handler;

    public GetPatientByIdQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _contextMock.SetupQueryType<PatientDetailsResponse>();
        _handler = new GetPatientByIdQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Patient_Does_Not_Exist()
    {
        // Arrange
        var patients = new List<Patient>().AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var query = new GetPatientByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(PatientErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Details_When_Found()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Id = patientId,
            FirstName = "Karim",
            MiddleName = "Ahmed",
            LastName = "Ali",
            PhoneNumber = "01000000000",
            DateOfBirth = new DateOnly(1990, 5, 10),
            Gender = Gender.Male,
            BloodType = BloodType.ABPositive,
            EmergencyContact = "01100000000",
            ApplicationUserId = Guid.NewGuid()
        };
        var patients = new List<Patient> { patient }.AsQueryable();
        _contextMock.Setup(c => c.Patients).Returns(patients);

        var query = new GetPatientByIdQuery(patientId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(patientId);
        result.Data.FullName.Should().Be("Karim Ahmed Ali");
        result.Data.IsAccountLinked.Should().BeTrue();
    }
}
