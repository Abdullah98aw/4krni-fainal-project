using System.Net.Mail;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Thakkirni.API.Application.Common.Exceptions;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.Data;
using Thakkirni.API.DTOs;
using Thakkirni.API.Models;
using Thakkirni.API.Services;

namespace Thakkirni.API.Application.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await QueryUsers()
            .OrderBy(u => u.Name)
            .Select(MapProjection())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserDto>> GetScopedUsersAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default)
    {
        var query = QueryUsers();
        if (currentRole != "ADMIN")
        {
            var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
            if (currentUser == null)
                return Array.Empty<UserDto>();

            if (currentUser.SectionId.HasValue)
                query = query.Where(u => u.Id == currentUserId || u.SectionId == currentUser.SectionId.Value);
            else if (currentUser.DepartmentId.HasValue)
                query = query.Where(u => u.Id == currentUserId || u.DepartmentId == currentUser.DepartmentId.Value);
            else if (currentUser.AgencyId.HasValue)
                query = query.Where(u => u.Id == currentUserId || u.AgencyId == currentUser.AgencyId.Value);
            else
                query = query.Where(u => u.Id == currentUserId);
        }

        return await query.OrderBy(u => u.Name).Select(MapProjection()).ToListAsync(cancellationToken);
    }

    public async Task<UserDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await QueryUsers().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            throw new ApiException(StatusCodes.Status404NotFound, "المستخدم غير موجود", new { errors = new[] { "المستخدم غير موجود" } });

        return MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        ValidateDto(dto, requirePassword: true);

        var normalizedEmail = NormalizeEmail(dto.Email) ?? throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "البريد الإلكتروني غير صالح" } });
        if (await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail, cancellationToken))
            throw new ApiException(StatusCodes.Status400BadRequest, "تعارض بيانات", new { errors = new[] { "البريد الإلكتروني مستخدم بالفعل" } });

        await EnsureJobTitleExists(dto.JobTitle, cancellationToken);

        var normalizedNationalId = NormalizeNationalId(dto.NationalId) ?? throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "رقم الهوية يجب أن يتكون من 10 أرقام فقط" } });
        if (await _context.Users.AnyAsync(u => u.NationalId == normalizedNationalId, cancellationToken))
            throw new ApiException(StatusCodes.Status400BadRequest, "تعارض بيانات", new { errors = new[] { "رقم الهوية مستخدم بالفعل" } });

        var orgSelection = await NormalizeOrgSelectionAsync(dto.AgencyId, dto.DepartmentId, dto.SectionId, cancellationToken);

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = normalizedEmail,
            NationalId = normalizedNationalId,
            Role = dto.Role,
            PasswordHash = PasswordHasherService.HashPassword(dto.Password!),
            Avatar = dto.Avatar ?? string.Empty,
            JobTitle = dto.JobTitle?.Trim() ?? string.Empty,
            AgencyId = orgSelection.AgencyId,
            DepartmentId = orgSelection.DepartmentId,
            SectionId = orgSelection.SectionId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetUserByIdAsync(user.Id, cancellationToken);
    }

    public async Task<UserDto> UpdateUserAsync(int id, CreateUpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        ValidateDto(dto, requirePassword: false);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            throw new ApiException(StatusCodes.Status404NotFound, "المستخدم غير موجود", new { errors = new[] { "المستخدم غير موجود" } });

        var normalizedEmail = NormalizeEmail(dto.Email) ?? throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "البريد الإلكتروني غير صالح" } });
        if (await _context.Users.AnyAsync(u => u.Id != id && u.Email != null && u.Email.ToLower() == normalizedEmail, cancellationToken))
            throw new ApiException(StatusCodes.Status400BadRequest, "تعارض بيانات", new { errors = new[] { "البريد الإلكتروني مستخدم بالفعل" } });

        await EnsureJobTitleExists(dto.JobTitle, cancellationToken);

        var normalizedNationalId = NormalizeNationalId(dto.NationalId) ?? throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "رقم الهوية يجب أن يتكون من 10 أرقام فقط" } });
        if (await _context.Users.AnyAsync(u => u.Id != id && u.NationalId == normalizedNationalId, cancellationToken))
            throw new ApiException(StatusCodes.Status400BadRequest, "تعارض بيانات", new { errors = new[] { "رقم الهوية مستخدم بالفعل" } });

        var orgSelection = await NormalizeOrgSelectionAsync(dto.AgencyId, dto.DepartmentId, dto.SectionId, cancellationToken);

        user.Name = dto.Name.Trim();
        user.Email = normalizedEmail;
        user.NationalId = normalizedNationalId;
        user.Role = dto.Role;
        user.Avatar = dto.Avatar ?? string.Empty;
        user.JobTitle = dto.JobTitle?.Trim() ?? string.Empty;
        user.AgencyId = orgSelection.AgencyId;
        user.DepartmentId = orgSelection.DepartmentId;
        user.SectionId = orgSelection.SectionId;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = PasswordHasherService.HashPassword(dto.Password);

        await _context.SaveChangesAsync(cancellationToken);
        return await GetUserByIdAsync(id, cancellationToken);
    }

    public async Task DeleteUserAsync(int id, int currentUserId, CancellationToken cancellationToken = default)
    {
        if (id == currentUserId)
            throw new ApiException(StatusCodes.Status400BadRequest, "عملية غير مسموحة", new { errors = new[] { "لا يمكن حذف المستخدم الحالي" } });

        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        if (user == null)
            throw new ApiException(StatusCodes.Status404NotFound, "المستخدم غير موجود", new { errors = new[] { "المستخدم غير موجود" } });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<User> QueryUsers() => _context.Users
        .Include(u => u.Agency)
        .Include(u => u.Department)
        .Include(u => u.Section)
        .AsSplitQuery()
        .AsNoTracking();

    private static Expression<Func<User, UserDto>> MapProjection() => u => new UserDto
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        NationalId = u.NationalId,
        Role = u.Role,
        Avatar = u.Avatar,
        JobTitle = u.JobTitle,
        AgencyId = u.AgencyId,
        AgencyName = u.Agency != null ? u.Agency.Name : null,
        DepartmentId = u.DepartmentId,
        DepartmentName = u.Department != null ? u.Department.Name : null,
        SectionId = u.SectionId,
        SectionName = u.Section != null ? u.Section.Name : null
    };

    private static UserDto MapToDto(User user) => new UserDto
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        NationalId = user.NationalId,
        Role = user.Role,
        Avatar = user.Avatar,
        JobTitle = user.JobTitle,
        AgencyId = user.AgencyId,
        AgencyName = user.Agency != null ? user.Agency.Name : null,
        DepartmentId = user.DepartmentId,
        DepartmentName = user.Department != null ? user.Department.Name : null,
        SectionId = user.SectionId,
        SectionName = user.Section != null ? user.Section.Name : null
    };

    private static void ValidateDto(CreateUpdateUserDto dto, bool requirePassword)
    {
        if (dto == null)
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "بيانات المستخدم غير صالحة" } });
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "اسم المستخدم مطلوب" } });
        if (requirePassword && string.IsNullOrWhiteSpace(dto.Password))
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "كلمة المرور مطلوبة عند إنشاء المستخدم" } });
        if (!IsAllowedRole(dto.Role))
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "الدور يجب أن يكون ADMIN أو MANAGER أو USER" } });
    }

    private static bool IsAllowedRole(string role) => role == "ADMIN" || role == "MANAGER" || role == "USER";

    private static string? NormalizeNationalId(string? nationalId)
    {
        var value = nationalId?.Trim();
        if (string.IsNullOrWhiteSpace(value)) return null;
        return Regex.IsMatch(value, @"^\d{10}$") ? value : null;
    }

    private static string? NormalizeEmail(string? email)
    {
        var value = email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(value)) return null;
        try { return new MailAddress(value).Address.ToLowerInvariant(); }
        catch { return null; }
    }

    private async Task EnsureJobTitleExists(string? jobTitle, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(jobTitle)) return;
        var exists = await _context.JobTitles.AnyAsync(j => j.Name == jobTitle, cancellationToken);
        if (!exists)
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "المسمى الوظيفي المحدد غير موجود في النظام" } });
    }

    private async Task<(int? AgencyId, int? DepartmentId, int? SectionId)> NormalizeOrgSelectionAsync(int? agencyId, int? departmentId, int? sectionId, CancellationToken cancellationToken)
    {
        var normalizedAgencyId = agencyId.HasValue && agencyId > 0 ? agencyId : null;
        var normalizedDepartmentId = departmentId.HasValue && departmentId > 0 ? departmentId : null;
        var normalizedSectionId = sectionId.HasValue && sectionId > 0 ? sectionId : null;

        Section? section = null;
        Department? department = null;

        if (normalizedSectionId.HasValue)
        {
            section = await _context.Sections.AsNoTracking().FirstOrDefaultAsync(s => s.Id == normalizedSectionId.Value, cancellationToken);
            if (section == null)
                throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "الشعبة المحددة غير موجودة" } });
            normalizedDepartmentId = section.DepartmentId;
        }

        if (normalizedDepartmentId.HasValue)
        {
            department = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == normalizedDepartmentId.Value, cancellationToken);
            if (department == null)
                throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "الإدارة المحددة غير موجودة" } });
            normalizedAgencyId = department.AgencyId;
        }

        if (normalizedAgencyId.HasValue)
        {
            var agencyExists = await _context.Agencies.AsNoTracking().AnyAsync(a => a.Id == normalizedAgencyId.Value, cancellationToken);
            if (!agencyExists)
                throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "الوكالة المحددة غير موجودة" } });
        }

        if (department != null && normalizedAgencyId != department.AgencyId)
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "الإدارة المحددة لا تنتمي إلى الوكالة المختارة" } });
        if (section != null && normalizedDepartmentId != section.DepartmentId)
            throw new ApiException(StatusCodes.Status400BadRequest, "بيانات غير صالحة", new { errors = new[] { "الشعبة المحددة لا تنتمي إلى الإدارة المختارة" } });

        return (normalizedAgencyId, normalizedDepartmentId, normalizedSectionId);
    }
}
