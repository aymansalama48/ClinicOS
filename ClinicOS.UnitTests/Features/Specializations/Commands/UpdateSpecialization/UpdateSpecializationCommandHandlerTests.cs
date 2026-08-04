using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Specializations;
using ClinicOS.Application.Features.Specializations.Commands.UpdateSpecialization;
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

namespace ClinicOS.UnitTests.Features.Specializations.Commands.UpdateSpecialization;

public class UpdateSpecializationCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly UpdateSpecializationCommandHandler _handler;

    public UpdateSpecializationCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _handler = new UpdateSpecializationCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Specialization_When_Valid()
    {
        // Arrange
        var specializationId = Guid.NewGuid();
        var existing = new Specialization("Neurology", "Old description") { Id = specializationId };
        var specializations = new List<Specialization> { existing }.AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(specializations);

        var command = new UpdateSpecializationCommand(specializationId, "Neurology Updated", "New description");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(specializationId);

        existing.Name.Should().Be(command.Name);
        existing.Description.Should().Be(command.Description);

        _contextMock.Verify(c => c.Update(existing), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Duplicate_Name_Exists()
    {
        // Arrange
        var specializationId = Guid.NewGuid();
        var existing1 = new Specialization("Cardiology", "Desc 1") { Id = specializationId };
        var existing2 = new Specialization("Pediatrics", "Desc 2") { Id = Guid.NewGuid() };
        var specializations = new List<Specialization> { existing1, existing2 }.AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(specializations);

        var command = new UpdateSpecializationCommand(specializationId, "Pediatrics", "Desc 1 updated");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SpecializationErrors.DuplicateName);

        _contextMock.Verify(c => c.Update(It.IsAny<Specialization>()), Times.Never);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Specialization_Not_Found()
    {
        // Arrange
        var specializations = new List<Specialization>().AsQueryable();
        _contextMock.Setup(c => c.Specializations).Returns(specializations);

        var command = new UpdateSpecializationCommand(Guid.NewGuid(), "Neurology", "Description");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(SpecializationErrors.NotFound);

        _contextMock.Verify(c => c.Update(It.IsAny<Specialization>()), Times.Never);
    }
}
