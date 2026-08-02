using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Errors.Specializations;

public static class SpecializationErrors
{
    public static readonly Error NotFound = new(
        "SPECIALIZATION_NOT_FOUND",
        "التخصص المطلوب غير موجود.",
        ErrorType.NotFound);

    public static readonly Error DuplicateName = new(
        "SPECIALIZATION_DUPLICATE_NAME",
        "يوجد تخصص آخر مسجل بنفس الاسم.",
        ErrorType.Conflict);
}