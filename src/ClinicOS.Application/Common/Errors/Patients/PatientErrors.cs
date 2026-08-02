using ClinicOS.Domain.Common.Results;

namespace ClinicOS.Application.Common.Errors.Patients;

public static class PatientErrors
{
    public static readonly Error PhoneNumberAlreadyExists = new(
        "PATIENT_PHONE_EXISTS",
        "رقم الهاتف مسجل لمريض آخر بالفعل في النظام.",
        ErrorType.Conflict);

    public static readonly Error NotFound = new(
        "PATIENT_NOT_FOUND",
        "المريض المطلوب غير موجود.",
        ErrorType.NotFound);
}