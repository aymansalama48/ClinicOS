using ClinicOS.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Domain.Entities.Prescriptions
{

    /// <summary>
    /// بند دواء داخل الروشتة
    /// </summary>
    public class PrescriptionItem : SoftDeleteEntity
    {
        public Guid PrescriptionId { get; set; }
        public Prescription Prescription { get; set; } = null!;

        // اسم الدواء
        public string MedicineName { get; set; } = string.Empty;

        // الجرعة
        public string? Dosage { get; set; }

        // عدد مرات الاستخدام
        public string? Frequency { get; set; }

        // مدة الاستخدام
        public string? Duration { get; set; }

        // ملاحظات إضافية
        public string? Instructions { get; set; }
    }
}
