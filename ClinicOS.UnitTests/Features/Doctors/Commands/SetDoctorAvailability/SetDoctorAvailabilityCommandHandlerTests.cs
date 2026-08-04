using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Features.Doctors.Commands.SetDoctorAvailability;
using ClinicOS.Domain.Entities.Doctors;
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

namespace ClinicOS.UnitTests.Features.Doctors.Commands.SetDoctorAvailability;

public class SetDoctorAvailabilityCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly SetDoctorAvailabilityCommandHandler _handler;

    public SetDoctorAvailabilityCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _handler = new SetDoctorAvailabilityCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Doctor_Does_Not_Exist()
    {
        // Arrange
        var doctors = new List<Doctor>().AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var command = new SetDoctorAvailabilityCommand(
            Guid.NewGuid(),
            DayOfWeek.Monday,
            PeriodType.Morning,
            new TimeOnly(9, 0),
            new TimeOnly(12, 0),
            10);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.NotFound);

        _contextMock.Verify(c => c.Add(It.IsAny<DoctorAvailability>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Conflict_When_Slot_Overlaps()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = Doctor.Create(Guid.NewGuid(), Guid.NewGuid(), "Bio", 4, 150, 20);
        doctor.Id = doctorId;
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var existingSlot = DoctorAvailability.Create(
            doctorId,
            DayOfWeek.Monday,
            PeriodType.Morning,
            new TimeOnly(8, 0),
            new TimeOnly(11, 0),
            8);

        var availabilities = new List<DoctorAvailability> { existingSlot }.AsQueryable();
        _contextMock.Setup(c => c.DoctorAvailabilities).Returns(availabilities);

        var command = new SetDoctorAvailabilityCommand(
            doctorId,
            DayOfWeek.Monday,
            PeriodType.Morning,
            new TimeOnly(9, 0),
            new TimeOnly(12, 0),
            10);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.AvailabilityConflict);

        _contextMock.Verify(c => c.Add(It.IsAny<DoctorAvailability>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Create_Availability_When_Valid()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = Doctor.Create(Guid.NewGuid(), Guid.NewGuid(), "Bio", 4, 150, 20);
        doctor.Id = doctorId;
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var availabilities = new List<DoctorAvailability>().AsQueryable();
        _contextMock.Setup(c => c.DoctorAvailabilities).Returns(availabilities);

        var command = new SetDoctorAvailabilityCommand(
            doctorId,
            DayOfWeek.Tuesday,
            PeriodType.Evening,
            new TimeOnly(17, 0),
            new TimeOnly(21, 0),
            15);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _contextMock.Verify(c => c.Add(It.Is<DoctorAvailability>(da =>
            da.DoctorId == doctorId &&
            da.DayOfWeek == DayOfWeek.Tuesday &&
            da.Period == PeriodType.Evening)), Times.Once);

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
