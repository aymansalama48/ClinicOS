namespace ClinicOS.Application.Common.Abstractions.Identity.Tokens;

public interface IJwtTokenGenerator
{
    /// توليد JWT قصير المدى خاص بالمريض (بدون Roles/Permissions)
    string GeneratePatientToken(
        Guid patientId,
        string phoneNumber,
        string? fullName = null);   // 👈 مضافة

    /// توليد JWT خاص بموظفي الداش بورد (Roles + Permissions + التخصص لو Doctor/Receptionist)
    string GenerateStaffToken(
        Guid userId,
        string email,
        string fullName,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        Guid? specializationId = null);
}