using ClinicOS.Infrastructure.Persistence.Data;
using ClinicOS.Infrastructure.Persistence.IdentityModels;
using ClinicOS.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.DependencyInjection;

public static partial class DependencyInjection
{
    /// <summary>
    /// تشغيل عملية الـ Seeding للـ Database والـ Roles والـ Permissions عند بداية التطبيق
    /// </summary>
    public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. تطبيق أي Migrations معلقة تلقائياً
            await dbContext.Database.MigrateAsync();
            // 👇 ضيف السطرين دول للتشخيص
            var actualConnectionString = dbContext.Database.GetConnectionString();
            Console.WriteLine("\n=================================================");
            Console.WriteLine($"🔥 THE API IS ACTUALLY CONNECTED TO: {actualConnectionString}");
            Console.WriteLine("=================================================\n");
            // 2. تشغيل الـ Seed الخاص بالـ Roles والـ Permissions
            await ContextSeed.SeedRolesAndPermissionsAsync(roleManager, dbContext);

            // 3. تشغيل الـ Seed الخاص بإنشاء مستخدم الـ Admin الافتراضي
            await ContextSeed.SeedDefaultAdminAsync(userManager, roleManager);

            logger.LogInformation("Database Seeding executed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}