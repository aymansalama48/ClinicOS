namespace ClinicOS.Application.Common.Abstractions.External.FileStorage;

public record StoredFile(
    string FileId,
    string? WebViewLink,
    long SizeInBytes
);
