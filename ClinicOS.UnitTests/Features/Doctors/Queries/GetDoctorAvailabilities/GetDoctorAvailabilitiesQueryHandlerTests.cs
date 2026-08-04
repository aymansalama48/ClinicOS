using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Doctors;
using ClinicOS.Application.Common.Pagination;
using ClinicOS.Application.Features.Doctors.Queries.GetDoctorAvailabilities;
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

namespace ClinicOS.UnitTests.Features.Doctors.Queries.GetDoctorAvailabilities;

public class GetDoctorAvailabilitiesQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly GetDoctorAvailabilitiesQueryHandler _handler;

    public GetDoctorAvailabilitiesQueryHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _contextMock.SetupQueryType<DoctorAvailabilityResponse>();
        _handler = new GetDoctorAvailabilitiesQueryHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Doctor_Does_Not_Exist()
    {
        // Arrange
        var doctors = new List<Doctor>().AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var query = new GetDoctorAvailabilitiesQuery(Guid.NewGuid(), null, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(DoctorErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Availabilities_When_Doctor_Exists()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = Doctor.Create(Guid.NewGuid(), Guid.NewGuid(), "Bio", 5, 200, 50);
        doctor.Id = doctorId;
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var slot1 = DoctorAvailability.Create(doctorId, DayOfWeek.Sunday, PeriodType.Morning, new TimeOnly(9, 0), new TimeOnly(13, 0), 10);
        var slot2 = DoctorAvailability.Create(doctorId, DayOfWeek.Monday, PeriodType.Evening, new TimeOnly(16, 0), new TimeOnly(20, 0), 12);
        var slots = new List<DoctorAvailability> { slot1, slot2 }.AsQueryable();
        _contextMock.Setup(c => c.DoctorAvailabilities).Returns(slots);

        var query = new GetDoctorAvailabilitiesQuery(doctorId, null, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.Pagination.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_Filter_By_DayOfWeek()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = Doctor.Create(Guid.NewGuid(), Guid.NewGuid(), "Bio", 5, 200, 50);
        doctor.Id = doctorId;
        var doctors = new List<Doctor> { doctor }.AsQueryable();
        _contextMock.Setup(c => c.Doctors).Returns(doctors);

        var slot1 = DoctorAvailability.Create(doctorId, DayOfWeek.Sunday, PeriodType.Morning, new TimeOnly(9, 0), new TimeOnly(13, 0), 10);
        var slot2 = DoctorAvailability.Create(doctorId, DayOfWeek.Monday, PeriodType.Evening, new TimeOnly(16, 0), new TimeOnly(20, 0), 12);
        var slots = new List<DoctorAvailability> { slot1, slot2 }.AsQueryable();
        _contextMock.Setup(c => c.DoctorAvailabilities).Returns(slots);

        var query = new GetDoctorAvailabilitiesQuery(doctorId, DayOfWeek.Sunday, new PaginationParameters { PageNumber = 1, PageSize = 10 });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().DayOfWeek.Should().Be(DayOfWeek.Sunday);
    }
}
