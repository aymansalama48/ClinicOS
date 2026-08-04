using ClinicOS.Application.Common.Abstractions.Identity.UserManagement;
using ClinicOS.Application.Common.Abstractions.Persistence;
using ClinicOS.Application.Common.Errors.Receptionists;
using ClinicOS.Application.Features.Receptionists.Commands.DeleteReceptionist;
using ClinicOS.Domain.Entities.Receptionists;
using ClinicOS.UnitTests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Receptionists.Commands.DeleteReceptionist;

public class DeleteReceptionistCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IUserManagementService> _userServiceMock;
    private readonly DeleteReceptionistCommandHandler _handler;

    public DeleteReceptionistCommandHandlerTests()
    {
        _contextMock = MockDbContextHelper.CreateBaseMock();
        _userServiceMock = new Mock<IUserManagementService>();
        _handler = new DeleteReceptionistCommandHandler(_contextMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Receptionist_Not_Found()
    {
        // Arrange
        var receptionists = new List<Receptionist>().AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var command = new DeleteReceptionistCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(ReceptionistErrors.NotFound);

        _contextMock.Verify(c => c.Remove(It.IsAny<Receptionist>()), Times.Never);
        _userServiceMock.Verify(u => u.DeactivateUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Delete_And_DeactivateUser_When_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var receptionist = Receptionist.Create(userId, Guid.NewGuid());
        var receptionists = new List<Receptionist> { receptionist }.AsQueryable();
        _contextMock.Setup(c => c.Receptionists).Returns(receptionists);

        var command = new DeleteReceptionistCommand(receptionist.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        _contextMock.Verify(c => c.Remove(receptionist), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _userServiceMock.Verify(u => u.DeactivateUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
