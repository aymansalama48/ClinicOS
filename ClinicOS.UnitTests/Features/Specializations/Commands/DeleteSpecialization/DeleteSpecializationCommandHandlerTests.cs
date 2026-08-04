using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Application.Features.Specializations.Commands.DeleteSpecialization;
using ClinicOS.Domain.Entities.Specializations;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Specializations.Commands.DeleteSpecialization;

public class DeleteSpecializationCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly DeleteSpecializationCommandHandler _handler;

    public DeleteSpecializationCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _handler = new DeleteSpecializationCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Delete_Specialization_When_Found()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existing = new Specialization("Dermatology", "Skin care") { Id = id };
        var specializations = new List<Specialization> { existing }.AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(specializations);

        var command = new DeleteSpecializationCommand(id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _contextMock.Verify(c => c.Remove(existing), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Specialization_Not_Found()
    {
        // Arrange
        var specializations = new List<Specialization>().AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(specializations);

        var command = new DeleteSpecializationCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SpecializationErrors.NotFound);

        _contextMock.Verify(c => c.Remove(It.IsAny<Specialization>()), Times.Never);
    }
}
