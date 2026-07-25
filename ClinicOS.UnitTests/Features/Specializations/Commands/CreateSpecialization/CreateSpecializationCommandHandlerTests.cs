using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;
using ClinicOS.Domain.Entities.Specializations;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Timers;
using Xunit;
using FluentAssertions;

namespace ClinicOS.UnitTests.Features.Specializations.Commands.CreateSpecialization;

public class CreateSpecializationCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<DbSet<Specialization>> _specializationsDbSetMock;
    private readonly CreateSpecializationCommandHandler _handler;

    public CreateSpecializationCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _specializationsDbSetMock = new Mock<DbSet<Specialization>>();

        // ربط DbSet الخاص بالتخصصات بالـ DbContext الوهمي
        _contextMock.Setup(c => c.Specializations).Returns(_specializationsDbSetMock.Object);

        // محاكاة حفظ التغييرات بنجاح
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
        result.Data.Should().NotBeEmpty();

        // التأكد من إضافة الكيان للـ DbSet بالشكل الصحيح
        _specializationsDbSetMock.Verify(
            s => s.Add(It.Is<Specialization>(sp => sp.Name == command.Name && sp.Description == command.Description)),
            Times.Once);

        // التأكد من حفظ التغييرات في قاعدة البيانات
        _contextMock.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}