using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicOS.Infrastructure.Persistence.Data
{
    /// <summary>
    /// مصنع لإنشاء DbContext في وقت التصميم (لأوامر Migration)
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(
                "Data Source=AYMAN\\MSSQLSERVER01;Database=ClinicOS;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;Packet Size=4096;Command Timeout=0");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}