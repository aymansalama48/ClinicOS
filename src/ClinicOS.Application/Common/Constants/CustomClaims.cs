namespace ClinicOS.Application.Common.Constants
{
    /// <summary>
    /// أسماء الـ Claims المستخدمة في JWT Token
    /// </summary>
    public static class CustomClaims
    {
        /// <summary>
        /// معرف المستخدم (UserId) - موجودة في توكن الـ Staff والمريض معًا
        /// </summary>
        public const string UserId = "userId";

        /// <summary>
        /// معرف المريض - موجودة في توكن المريض فقط (لو موجودة، يبقى التوكن ده توكن مريض مش Staff)
        /// </summary>
        public const string PatientId = "patientId";

        /// <summary>
        /// معرف التخصص - موجودة في توكن الـ Doctor والـ Receptionist فقط، null للـ Admin
        /// </summary>
        public const string SpecializationId = "specializationId";

        /// <summary>
        /// الدور (Role) - يمكن استخدامه كاختصار للـ Role الرئيسي
        /// </summary>
        public const string Role = "role";

        /// <summary>
        /// الصلاحية (Permission) - تُستخدم في RBAC الدقيق
        /// </summary>
        public const string Permission = "permission";

        // يمكن إضافة ثوابت أخرى حسب الحاجة، مثل:
        // public const string Email = "email";
        // public const string FullName = "fullName";
    }
}