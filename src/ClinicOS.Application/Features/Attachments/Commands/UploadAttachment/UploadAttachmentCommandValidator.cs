using FluentValidation;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using ClinicOS.Application.Common.Options;
using ClinicOS.Application.Common.Abstractions.Attachments;

namespace ClinicOS.Application.Features.Attachments.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    private static readonly byte[] PdfMagicBytes = { 0x25, 0x50, 0x44, 0x46 }; // %PDF
    private static readonly byte[] JpegMagicBytes = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] PngMagicBytes = { 0x89, 0x50, 0x4E, 0x47 };

    public UploadAttachmentCommandValidator(
        IOptions<AttachmentSettings> options,
        IAttachmentEntityResolver entityResolver)
    {
        var settings = options.Value;
        var maxBytes = settings.MaxSizeInMB * 1024 * 1024;

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("اسم الملف مطلوب.");
            
        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("نوع الملف مطلوب.");

        RuleFor(x => x.EntityType)
            .NotEmpty().WithMessage("نوع الكيان (EntityType) مطلوب.")
            .Must(type => entityResolver.TryResolve(type, out _))
            .WithMessage("نوع الكيان غير مدعوم للمرفقات.");

        RuleFor(x => x.Size)
            .GreaterThan(0).WithMessage("حجم الملف غير صالح.")
            .LessThanOrEqualTo(maxBytes).WithMessage($"حجم الملف يجب ألا يتجاوز {settings.MaxSizeInMB} ميجابايت.");

        RuleFor(x => x.Content)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("محتوى الملف مطلوب.")
            .Must(c => c.Length > 0).WithMessage("الملف فارغ.")
            .Must(IsValidFile).WithMessage("محتوى الملف غير مدعوم أو تالف بناءً على بنيته (Magic Bytes).")
            .Must(c => 
            {
                if (c.CanSeek) c.Position = 0;
                return true;
            });
    }

    private bool IsValidFile(Stream stream)
    {
        if (stream == null || !stream.CanRead) return false;
        
        if (stream.CanSeek) stream.Position = 0;
        
        var buffer = new byte[4];
        var bytesRead = stream.Read(buffer, 0, 4);
        
        if (stream.CanSeek) stream.Position = 0; // إرجاع المؤشر للصفر
        
        if (bytesRead < 3) return false;

        if (buffer.Take(4).SequenceEqual(PdfMagicBytes)) return true;
        if (buffer.Take(3).SequenceEqual(JpegMagicBytes)) return true;
        if (buffer.Take(4).SequenceEqual(PngMagicBytes)) return true;

        return false;
    }
}