using ClinicOS.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Domain.Entities.Invitation.Events
{
    public sealed record StaffInvitationCreatedEvent(StaffInvitation Invitation) : IDomainEvent;
}
