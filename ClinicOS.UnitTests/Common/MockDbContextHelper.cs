using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Domain.Entities.Doctors;
using ClinicOS.Domain.Entities.Patients;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.Domain.Entities.Specializations;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.UnitTests.Common;

public static class MockDbContextHelper
{
    public static Mock<IApplicationDbContext> CreateBaseMock()
    {
        var mock = new Mock<IApplicationDbContext>();

        mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mock.SetupCommonEntity<Specialization>();
        mock.SetupCommonEntity<Doctor>();
        mock.SetupCommonEntity<DoctorAvailability>();
        mock.SetupCommonEntity<Receptionist>();
        mock.SetupCommonEntity<Patient>();

        return mock;
    }

    public static Mock<IApplicationDbContext> SetupCommonEntity<T>(this Mock<IApplicationDbContext> mock) where T : class
    {
        mock.Setup(c => c.AsNoTracking(It.IsAny<IQueryable<T>>()))
            .Returns<IQueryable<T>>(q => q);

        mock.Setup(c => c.AnyAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.Any());

        mock.Setup(c => c.FirstOrDefaultAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.FirstOrDefault());

        mock.Setup(c => c.CountAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.Count());

        mock.Setup(c => c.ToListAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.ToList());

        return mock;
    }

    public static Mock<IApplicationDbContext> SetupQueryType<T>(this Mock<IApplicationDbContext> mock)
    {
        mock.Setup(c => c.AnyAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.Any());

        mock.Setup(c => c.FirstOrDefaultAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.FirstOrDefault());

        mock.Setup(c => c.CountAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.Count());

        mock.Setup(c => c.ToListAsync(It.IsAny<IQueryable<T>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<T> q, CancellationToken ct) => q.ToList());

        return mock;
    }
}
