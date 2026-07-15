using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Errors.Files;

/// <summary>
/// يجمع أخطاء إدارة الملفات المرفقة (صور الأشعة، التقارير الطبية، إلخ).
/// </summary>
public static class FileErrors
{
    public static readonly Error FileNotFound = new(
        "FILE_NOT_FOUND",
        "The requested file was not found.",
        ErrorType.NotFound);

    public static readonly Error EmptyFile = new(
        "FILE_EMPTY",
        "The uploaded file is empty.",
        ErrorType.Validation);

    public static readonly Error InvalidFileName = new(
        "FILE_INVALID_NAME",
        "The file name is invalid or empty.",
        ErrorType.Validation);

    public static readonly Error InvalidFolder = new(
        "FILE_INVALID_FOLDER",
        "The target folder path is invalid or does not exist.",
        ErrorType.Validation);

    public static readonly Error FileTooLarge = new(
        "FILE_TOO_LARGE",
        "The file size exceeds the maximum allowed limit.",
        ErrorType.Validation);

    public static readonly Error UnsupportedExtension = new(
        "FILE_UNSUPPORTED_EXTENSION",
        "The file extension/type is not supported.",
        ErrorType.Validation);

    public static readonly Error FileAlreadyExists = new(
        "FILE_ALREADY_EXISTS",
        "A file with the same name already exists.",
        ErrorType.Conflict);

    public static readonly Error UploadFailed = new(
        "FILE_UPLOAD_FAILED",
        "An unexpected error occurred while uploading the file.",
        ErrorType.Failure);
}