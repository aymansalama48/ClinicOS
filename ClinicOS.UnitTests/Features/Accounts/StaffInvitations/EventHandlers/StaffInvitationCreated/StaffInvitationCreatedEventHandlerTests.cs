using ClinicOS.Application.Common.Abstractions.External.Jobs;
using ClinicOS.Application.Common.Abstractions.Notifications;
using ClinicOS.Application.Common.Events;
using ClinicOS.Application.Features.Accounts.StaffInvitations.EventHandlers.StaffInvitationCreated;
using ClinicOS.Domain.Entities.Invitation;
using ClinicOS.Domain.Entities.Invitation.Events;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ClinicOS.UnitTests.Features.Accounts.StaffInvitations.EventHandlers.StaffInvitationCreated;

public class StaffInvitationCreatedEventHandlerTests
{
    private readonly Mock<IJobScheduler> _jobSchedulerMock;
    private readonly StaffInvitationCreatedEventHandler _handler;

    public StaffInvitationCreatedEventHandlerTests()
    {
        _jobSchedulerMock = new Mock<IJobScheduler>();
        _handler = new StaffInvitationCreatedEventHandler(_jobSchedulerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Enqueue_Invitation_Email()
    {
        // Arrange
        var invitation = StaffInvitation.Create(
            "doctor@clinic.com",
            "Doctor",
            "secure-token-123",
            DateTime.UtcNow.AddDays(2),
            Guid.NewGuid(),
            "Admin User",
            null);

        var domainEvent = new StaffInvitationCreatedEvent(invitation);
        var notification = new DomainEventNotification<StaffInvitationCreatedEvent>(domainEvent);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _jobSchedulerMock.Verify(j => j.Enqueue(It.IsAny<Expression<Func<IIdentityNotificationService, Task>>>()), Times.Once);
    }
}
