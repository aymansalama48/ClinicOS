using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Application.Features.Specializations.Commands.CreateSpecialization;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Specializations.Commands.CreateSpecialization;

public class CreateSpecializationCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly CreateSpecializationCommandHandler _handler;

    public CreateSpecializationCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _handler = new CreateSpecializationCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Specialization_And_Return_Success_Result()
    {
        // Arrange
        var specializations = new List<Specialization>().AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(specializations);

        var command = new CreateSpecializationCommand("Cardiology", "Heart and cardiovascular care");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _contextMock.Verify(
            c => c.Add(It.Is<Specialization>(sp =>
                sp.Name == command.Name &&
                sp.Description == command.Description)),
            Times.Once);

        _contextMock.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Name_Is_Duplicate()
    {
        // Arrange
        var existing = new Specialization("Cardiology", "Existing");
        var specializations = new List<Specialization> { existing }.AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(specializations);

        var command = new CreateSpecializationCommand("Cardiology", "New Description");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SpecializationErrors.DuplicateName);

        _contextMock.Verify(c => c.Add(It.IsAny<Specialization>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}