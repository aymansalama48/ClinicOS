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

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    private readonly IDateTime _dateTime;
    public JwtTokenGenerator(IOptions<JwtOptions> options, IDateTime dateTime)
    {
        _options = options.Value;
        _dateTime = dateTime;
    }
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

        return BuildToken(claims, _options.PatientExpiryMinutes);
    }

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

        return BuildToken(claims, _options.StaffExpiryMinutes);
    }

    private string BuildToken(List<Claim> claims, int expiryMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
expires: _dateTime.Now.AddMinutes(expiryMinutes), // أو _dateTime.UtcNow
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}