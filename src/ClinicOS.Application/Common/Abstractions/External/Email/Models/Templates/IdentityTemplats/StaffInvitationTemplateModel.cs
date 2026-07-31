using ClinicOS.Application.Common.Abstractions.External.Email.Models.Base;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Models.Templates.IdentityTemplats;

/// <summary>
/// نموذج بريد دعوة موظف جديد (Doctor, Receptionist, Admin)
/// </summary>
public class StaffInvitationTemplateModel : BaseEmailTemplateModel
{
    /// <summary>
    /// البريد الإلكتروني الذي تم إرسال الدعوة إليه (ليتم عرضه داخل الإيميل)
    /// </summary>
    public string InvitedEmail { get; set; } = string.Empty;

    /// <summary>
    /// اسم الأدمن الذي أرسل الدعوة
    /// </summary>
    public string AdminName { get; set; } = string.Empty;

    /// <summary>
    /// المسمى الوظيفي بالعربية (طبيب، موظف استقبال، مدير)
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// التوكن العشوائي الآمن الخاص بالدعوة
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// رابط قبول الدعوة (سيتم بناؤه ديناميكياً في السيرفس)
    /// </summary>
    public string InvitationLink { get; set; } = string.Empty;

    /// <summary>
    /// مدة صلاحية الرابط بالساعات (يتم حسابها من الـ Entity)
    /// </summary>
    public int ExpiryHours { get; set; }
}