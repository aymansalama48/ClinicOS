using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;
using ClinicOS.Domain.Entities.Specializations;
using Moq;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicOS.UnitTests.Features.Specializations.Commands.CreateSpecialization;

public class CreateSpecializationCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CreateSpecializationCommandHandler _handler;

    public CreateSpecializationCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();

        // 1. محاكاة الـ IQueryable (مبقاش في DbSet خلاص)
        var emptySpecializations = new List<Specialization>().AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(emptySpecializations);

        // 2. محاكاة دالة Add الجديدة اللي ضفناها في الواجهة
        _contextMock.Setup(c => c.Add(It.IsAny<Specialization>()));

        // 3. محاكاة حفظ التغييرات
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

        _handler = new CreateSpecializationCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Specialization_And_Return_Success_Result()
    {
        // Arrange
        var command = new CreateSpecializationCommand("Cardiology", "Heart and cardiovascular care");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // (ملاحظة: لو الـ Result بتاعك مش بيرجع Data، شيل السطر اللي تحت ده)
        // result.Data.Should().NotBeEmpty(); 

        // 4. التأكد من استدعاء دالة Add المباشرة من الـ Context
        _contextMock.Verify(
            c => c.Add(It.Is<Specialization>(sp =>
                sp.Name == command.Name &&
                sp.Description == command.Description)),
            Times.Once);

        // التأكد من حفظ التغييرات في قاعدة البيانات
        _contextMock.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}