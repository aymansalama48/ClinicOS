namespace ClinicOS.Infrastructure.External.Storage;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClinicOS.Application.Common.Abstractions.External.FileStorage;
using ClinicOS.Domain.Common.Results;
using ClinicOS.Infrastructure.Options;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using File = Google.Apis.Drive.v3.Data.File;

public sealed class GoogleDriveContentStorage : IFileStorage, IDisposable
{
    private readonly DriveService _driveService;
    private readonly GoogleDriveOptions _options;
    private readonly ILogger<GoogleDriveContentStorage> _logger;

    private readonly ConcurrentDictionary<string, string> _folderCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _folderLock = new(1, 1);

    public GoogleDriveContentStorage(
        IOptions<GoogleDriveOptions> options,
        ILogger<GoogleDriveContentStorage> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (!System.IO.File.Exists(_options.ServiceAccountFilePath))
        {
            _logger.LogWarning("Google Drive credentials file not found at {Path}. Google Drive storage will be disabled.", _options.ServiceAccountFilePath);
            return;
        }

#pragma warning disable CS0618
        var credential = GoogleCredential
            .FromFile(_options.ServiceAccountFilePath)
            .CreateScoped(DriveService.Scope.DriveFile);
#pragma warning restore CS0618

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "ClinicOS"
        });
    }

    public async Task<Result<StoredFile>> UploadAsync(Stream fileStream, string storedFileName, string folderName, string contentType, CancellationToken cancellationToken = default)
    {
        if (_driveService == null)
            return Result<StoredFile>.Failure(new Error("Drive.Disabled", "خدمة جوجل درايف غير مفعلة (ملف الصلاحيات مفقود).", ErrorType.Failure));

        try
        {
            var folderId = await GetOrCreateFolderAsync(folderName, cancellationToken);

            var fileMetadata = new File
            {
                Name = storedFileName,
                Parents = new List<string> { folderId }
            };

            var request = _driveService.Files.Create(fileMetadata, fileStream, contentType);
            request.Fields = "id, webViewLink, size";

            var progress = await request.UploadAsync(cancellationToken);

            if (progress.Status == UploadStatus.Failed)
            {
                _logger.LogError(progress.Exception, "Google Drive upload failed.");
                return Result<StoredFile>.Failure(new Error("Drive.UploadFailed", "فشل رفع الملف إلى جوجل درايف.", ErrorType.Failure));
            }

            var file = request.ResponseBody;
            long size = file.Size.HasValue ? file.Size.Value : fileStream.Length;

            return Result<StoredFile>.Success(new StoredFile(file.Id, file.WebViewLink, size));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Google Drive upload.");
            return Result<StoredFile>.Failure(new Error("Drive.UploadException", "حدث خطأ أثناء رفع الملف.", ErrorType.Failure));
        }
    }

    public async Task<Result<Stream>> DownloadAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (_driveService == null)
            return Result<Stream>.Failure(new Error("Drive.Disabled", "خدمة جوجل درايف غير مفعلة (ملف الصلاحيات مفقود).", ErrorType.Failure));

        try
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            
            await request.DownloadAsync(stream, cancellationToken);
            stream.Position = 0;
            
            return Result<Stream>.Success(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {FileId}", fileId);
            return Result<Stream>.Failure(new Error("Drive.DownloadFailed", "فشل تحميل الملف.", ErrorType.Failure));
        }
    }

    public async Task<Result> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (_driveService == null)
            return Result.Failure(new Error("Drive.Disabled", "خدمة جوجل درايف غير مفعلة (ملف الصلاحيات مفقود).", ErrorType.Failure));

        try
        {
            var request = _driveService.Files.Delete(fileId);
            await request.ExecuteAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {FileId}", fileId);
            return Result.Failure(new Error("Drive.DeleteFailed", "فشل حذف الملف.", ErrorType.Failure));
        }
    }

    public async Task<Result<bool>> ExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if (_driveService == null)
            return Result<bool>.Failure(new Error("Drive.Disabled", "خدمة جوجل درايف غير مفعلة (ملف الصلاحيات مفقود).", ErrorType.Failure));

        try
        {
            var request = _driveService.Files.Get(fileId);
            request.Fields = "id";
            var file = await request.ExecuteAsync(cancellationToken);
            return Result<bool>.Success(file != null);
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check existence for file {FileId}", fileId);
            return Result<bool>.Failure(new Error("Drive.ExistsFailed", "فشل التحقق من وجود الملف.", ErrorType.Failure));
        }
    }

    private async Task<string> GetOrCreateFolderAsync(string folderName, CancellationToken cancellationToken)
    {
        if (_folderCache.TryGetValue(folderName, out var cachedId))
            return cachedId;

        await _folderLock.WaitAsync(cancellationToken);
        try
        {
            if (_folderCache.TryGetValue(folderName, out cachedId))
                return cachedId;

            var escapedFolderName = folderName.Replace("'", "\\'");
            var query = $"mimeType='application/vnd.google-apps.folder' and name='{escapedFolderName}' and '{_options.RootFolderId}' in parents and trashed=false";
            
            var listRequest = _driveService.Files.List();
            listRequest.Q = query;
            listRequest.Fields = "files(id)";
            
            var listResponse = await listRequest.ExecuteAsync(cancellationToken);
            var folder = listResponse.Files?.FirstOrDefault();

            if (folder != null)
            {
                _folderCache[folderName] = folder.Id;
                return folder.Id;
            }

            var folderMetadata = new File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { _options.RootFolderId }
            };

            var createRequest = _driveService.Files.Create(folderMetadata);
            createRequest.Fields = "id";
            var newFolder = await createRequest.ExecuteAsync(cancellationToken);

            _folderCache[folderName] = newFolder.Id;
            return newFolder.Id;
        }
        finally
        {
            _folderLock.Release();
        }
    }

    public void Dispose()
    {
        _driveService?.Dispose();
        _folderLock?.Dispose();
    }
}
