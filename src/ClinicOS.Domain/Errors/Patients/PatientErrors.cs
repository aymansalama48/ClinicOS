using ClinicOS.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Domain.Errors.Patients
{
    /// <summary>
    /// أخطاء خاصة بعمليات المريض (Patient) في النظام
    /// </summary>
    public static class PatientErrors
    {
        /// <summary>
        /// المريض غير موجود
        /// </summary>
        public static readonly Error NotFound = new(
            "Patient.NotFound",
            "لم يتم العثور على المريض المطلوب",
            ErrorType.NotFound);

        /// <summary>
        /// رقم الهاتف غير مسجل في النظام
        /// </summary>
        public static readonly Error PhoneNotRegistered = new(
            "Patient.PhoneNotRegistered",
            "رقم الهاتف غير مسجل في النظام، يرجى إنشاء حجز جديد أولاً",
            ErrorType.Validation);

        /// <summary>
        /// المريض لديه حساب دائم بالفعل
        /// </summary>
        public static readonly Error AccountAlreadyLinked = new(
            "Patient.AccountAlreadyLinked",
            "هذا المريض لديه حساب دائم بالفعل (Email + Password)",
            ErrorType.Validation);

        /// <summary>
        /// المريض ليس لديه حساب دائم (لا يمكنه تسجيل الدخول بالبريد)
        /// </summary>
        public static readonly Error NoPermanentAccount = new(
            "Patient.NoPermanentAccount",
            "هذا المريض ليس لديه حساب دائم، يرجى إنشاء حساب أولاً",
            ErrorType.Validation);

        /// <summary>
        /// فشل في إنشاء الحساب الدائم للمريض
        /// </summary>
        public static readonly Error AccountCreationFailed = new(
            "Patient.AccountCreationFailed",
            "فشل في إنشاء الحساب الدائم، يرجى المحاولة مرة أخرى",
            ErrorType.Failure);

        /// <summary>
        /// رقم الهاتف مستخدم بالفعل من قبل مريض آخر
        /// </summary>
        public static readonly Error PhoneAlreadyExists = new(
            "Patient.PhoneAlreadyExists",
            "رقم الهاتف مستخدم بالفعل من قبل مريض آخر",
            ErrorType.Validation);

        /// <summary>
        /// البريد الإلكتروني مستخدم بالفعل من قبل مستخدم آخر
        /// </summary>
        public static readonly Error EmailAlreadyExists = new(
            "Patient.EmailAlreadyExists",
            "البريد الإلكتروني مستخدم بالفعل من قبل حساب آخر",
            ErrorType.Validation);

        /// <summary>
        /// بيانات المريض غير مكتملة
        /// </summary>
        public static readonly Error IncompleteData = new(
            "Patient.IncompleteData",
            "بيانات المريض غير مكتملة، يرجى إكمال البيانات المطلوبة",
            ErrorType.Validation);

        /// <summary>
        /// فشل في تحديث بيانات المريض
        /// </summary>
        public static Error UpdateFailed(string details) => new(
            "Patient.UpdateFailed",
            $"فشل في تحديث بيانات المريض: {details}",
            ErrorType.Failure);
    }
}
