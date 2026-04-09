using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Thakkirni.API.Application.Common.Exceptions;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.Data;
using Thakkirni.API.DTOs;
using Thakkirni.API.Services;

namespace Thakkirni.API.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = loginDto.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(loginDto.Password))
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { message = "البريد الإلكتروني وكلمة المرور مطلوبان" });

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Agency)
            .Include(u => u.Department)
            .Include(u => u.Section)
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail, cancellationToken);

        if (user == null || !PasswordHasherService.VerifyPassword(loginDto.Password, user.PasswordHash))
            throw new ApiException(StatusCodes.Status401Unauthorized, "فشل تسجيل الدخول", new { message = "بيانات الدخول غير صحيحة" });

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing."));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "USER"),
                new Claim("AgencyId", user.AgencyId?.ToString() ?? string.Empty),
                new Claim("DepartmentId", user.DepartmentId?.ToString() ?? string.Empty),
                new Claim("SectionId", user.SectionId?.ToString() ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                NationalId = user.NationalId,
                Role = user.Role,
                Avatar = user.Avatar,
                JobTitle = user.JobTitle,
                AgencyId = user.AgencyId,
                AgencyName = user.Agency?.Name,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name,
                SectionId = user.SectionId,
                SectionName = user.Section?.Name
            }
        };
    }
}
