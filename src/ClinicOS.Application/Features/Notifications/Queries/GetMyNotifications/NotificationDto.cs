namespace ClinicOS.Application.Features.Notifications.Queries.GetMyNotifications;

public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    string Type,
    bool IsRead,
    string? ActionUrl,
    DateTime CreatedAt
);
