namespace ClinicOS.Infrastructure.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        // المفتاح السري اللي بيتم توقيع التوكن بيه (لازم يكون طويل وقوي ومحفوظ بأمان)
        public string Key { get; set; } = string.Empty;

        // اسم الجهة اللي أصدرت التوكن (عادة اسم موقعك أو الـ API)
        public string Issuer { get; set; } = string.Empty;

        // اسم الجهة اللي التوكن موجه ليها (عادة الفرونت إند أو الموبايل أب)
        public string Audience { get; set; } = string.Empty;

        // عدد الدقايق اللي توكن الـ Staff يبقى صالح فيها (Admin/Doctor/Receptionist)
        public int StaffExpiryMinutes { get; set; } = 60;

        // عدد الدقايق اللي توكن المريض يبقى صالح فيها (قصير عمدًا لأنه من غير باسورد)
        public int PatientExpiryMinutes { get; set; } = 15;
    }
}