using ClinicOS.Application.Common.Abstractions.Core;
using ClinicOS.Application.Common.Abstractions.Identity.Tokens;
using ClinicOS.Application.Common.Constants;
using ClinicOS.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicOS.Infrastructure.Identity.Tokens;

/// <summary>
/// مولّد توكنات JWT — للمريض والـ Staff
/// </summary>
public class JwtTokenGenerator(
    IOptions<JwtOptions> options,
    IDateTime dateTime) : IJwtTokenGenerator
{
    /// <summary>
    /// توليد توكن المريض — بيعتمد على الـ PatientId ورقم الموبايل
    /// </summary>
    public string GeneratePatientToken(
        Guid patientId,
        string phoneNumber,
        string? fullName = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(CustomClaims.PatientId, patientId.ToString()),
            new(ClaimTypes.MobilePhone, phoneNumber)
        };

        if (!string.IsNullOrWhiteSpace(fullName))
            claims.Add(new Claim(ClaimTypes.Name, fullName));

        return BuildToken(claims, options.Value.PatientExpiryMinutes);
    }

    /// <summary>
    /// توليد توكن الـ Staff — بيعتمد على الـ UserId والأدوار والصلاحيات والتخصص
    /// </summary>
    public string GenerateStaffToken(
        Guid userId,
        string email,
        string fullName,
        IEnumerable<string> roles,
        IEnumerable<string> permissions,
        Guid? specializationId = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(CustomClaims.UserId, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, fullName)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        foreach (var permission in permissions)
            claims.Add(new Claim(CustomClaims.Permission, permission));

        if (specializationId.HasValue)
            claims.Add(new Claim(CustomClaims.SpecializationId, specializationId.Value.ToString()));

        return BuildToken(claims, options.Value.StaffExpiryMinutes);
    }

    // بناء التوكن النهائي بالـ Claims ومدة الصلاحية
    private string BuildToken(List<Claim> claims, int expiryMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            expires: dateTime.Now.AddMinutes(expiryMinutes), // أو dateTime.UtcNow
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}