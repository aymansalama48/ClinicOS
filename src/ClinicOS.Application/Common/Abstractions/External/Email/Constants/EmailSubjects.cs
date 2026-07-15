using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Common.Abstractions.External.Email.Constants
{
    public class EmailSubjectsConstants
    {
        public static string EmailConfirmation(string companyName) => $"تأكيد بريدك الإلكتروني - {companyName}";
        public static string ResetPassword(string companyName) => $"إعادة تعيين كلمة المرور - {companyName}";
        public static string Welcome(string companyName) => $"مرحباً بك في {companyName}";
        public static string PasswordChanged(string companyName) => $"تم تغيير كلمة المرور - {companyName}";
        public static string AccountLocked(string companyName) => $"تم قفل حسابك - {companyName}";
        public static string AccountUnlocked(string companyName) => $"تم فتح قفل حسابك - {companyName}";
        public static string RoleAssigned(string companyName) => $"تم تعيين دور جديد لك - {companyName}";
        public static string RoleRemoved(string companyName) => $"تم إزالة دورك - {companyName}";
        public static string CompanyInvite(string companyName) => $"دعوة للانضمام إلى {companyName}";
    }
}
