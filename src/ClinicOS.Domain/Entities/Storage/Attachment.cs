using ClinicOS.Domain.Common.Entities;
using System;

namespace ClinicOS.Domain.Entities.Storage;

/// <summary>
/// كيان يمثل الملفات المرفوعة (صور، مستندات) ويدعم التخزين الخارجي مثل Google Drive
/// </summary>
public class Attachment : SoftDeleteEntity
{
    // جميع الخصائص Private Set لمنع التعديل العشوائي
    public string FileName { get; private set; }
    public string StoredName { get; private set; }
    public string ContentType { get; private set; }
    public string FileId { get; private set; } // Google Drive File ID
    public string? ExternalUrl { get; private set; }
    public long SizeInBytes { get; private set; }

    // تم تحويلهم لـ Nullable لتسهيل عملية الرفع المستقلة
    public string? EntityType { get; private set; }
    public Guid? EntityId { get; private set; }

    // Constructor خاص بـ EF Core فقط
    private Attachment() { }

    private Attachment(string fileName, string storedName, string contentType, string fileId, long sizeInBytes, string? externalUrl, string? entityType, Guid? entityId)
    {
        FileName = fileName;
        StoredName = storedName;
        ContentType = contentType;
        FileId = fileId;
        SizeInBytes = sizeInBytes;
        ExternalUrl = externalUrl;
        EntityType = entityType;
        EntityId = entityId;
    }

    // Factory Method لإنشاء الكيان بطريقة آمنة
    public static Attachment Create(string fileName, string storedName, string contentType, string fileId, long sizeInBytes, string? externalUrl = null, string? entityType = null, Guid? entityId = null)
    {
        return new Attachment(fileName, storedName, contentType, fileId, sizeInBytes, externalUrl, entityType, entityId);
    }

    // دالة لربط الملف بكيان معين لاحقاً (مثلاً بعد إنشاء التخصص)
    public void AssignToEntity(string entityType, Guid entityId)
    {
        if (EntityId != null || EntityType != null)
        {
            throw new InvalidOperationException("This attachment is already assigned to an entity.");
        }
        
        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type cannot be null or empty", nameof(entityType));
            
        EntityType = entityType;
        EntityId = entityId;
    }
}