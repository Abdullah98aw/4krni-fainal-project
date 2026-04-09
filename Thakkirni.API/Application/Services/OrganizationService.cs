using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Application.Common.Exceptions;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.Data;
using Thakkirni.API.DTOs;
using Thakkirni.API.Models;

namespace Thakkirni.API.Application.Services;

public sealed class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;

    public OrganizationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetAgenciesAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default)
    {
        IQueryable<Agency> query = _context.Agencies.AsNoTracking();
        if (currentRole == "MANAGER")
        {
            var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
            if (currentUser?.AgencyId != null)
                query = query.Where(a => a.Id == currentUser.AgencyId.Value);
            else if (currentUser?.DepartmentId != null)
                query = query.Where(a => a.Departments.Any(d => d.Id == currentUser.DepartmentId.Value));
            else
                query = query.Take(0);
        }

        return await query.Select(a => new { a.Id, a.Name }).OrderBy(a => a.Name).ToListAsync(cancellationToken);
    }

    public async Task<object> CreateAgencyAsync(UpsertAgencyDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeName(dto.Name) ?? throw BadRequest("اسم الوكالة مطلوب");
        if (await _context.Agencies.AnyAsync(a => a.Name.ToLower() == normalizedName.ToLower(), cancellationToken))
            throw BadRequest("اسم الوكالة مستخدم بالفعل");

        var agency = new Agency { Name = normalizedName };
        _context.Agencies.Add(agency);
        await _context.SaveChangesAsync(cancellationToken);
        return new { agency.Id, agency.Name };
    }

    public async Task<object> UpdateAgencyAsync(int id, UpsertAgencyDto dto, CancellationToken cancellationToken = default)
    {
        var agency = await _context.Agencies.FindAsync(new object[] { id }, cancellationToken) ?? throw NotFound();
        var normalizedName = NormalizeName(dto.Name) ?? throw BadRequest("اسم الوكالة مطلوب");
        if (await _context.Agencies.AnyAsync(a => a.Name.ToLower() == normalizedName.ToLower() && a.Id != id, cancellationToken))
            throw BadRequest("اسم الوكالة مستخدم بالفعل");

        agency.Name = normalizedName;
        await _context.SaveChangesAsync(cancellationToken);
        return new { agency.Id, agency.Name };
    }

    public async Task DeleteAgencyAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await _context.Departments.AnyAsync(d => d.AgencyId == id, cancellationToken))
            throw BadRequest("لا يمكن حذف الوكالة لوجود إدارات مرتبطة بها");

        var agency = await _context.Agencies.FindAsync(new object[] { id }, cancellationToken) ?? throw NotFound();
        _context.Agencies.Remove(agency);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<object> GetDepartmentsAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default)
    {
        var query = _context.Departments.AsNoTracking().AsQueryable();
        if (currentRole == "MANAGER")
        {
            var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
            if (currentUser?.DepartmentId != null)
                query = query.Where(d => d.Id == currentUser.DepartmentId.Value);
            else if (currentUser?.AgencyId != null)
                query = query.Where(d => d.AgencyId == currentUser.AgencyId.Value);
            else
                query = query.Take(0);
        }

        return await query.Select(d => new { d.Id, d.Name, d.AgencyId, AgencyName = d.Agency.Name }).OrderBy(d => d.Name).ToListAsync(cancellationToken);
    }

    public async Task<object> GetDepartmentsByAgencyAsync(int agencyId, int currentUserId, string currentRole, CancellationToken cancellationToken = default)
    {
        var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        if (currentRole == "MANAGER" && currentUser != null)
        {
            if (currentUser.AgencyId.HasValue && currentUser.AgencyId != agencyId)
                throw new ApiException(StatusCodes.Status403Forbidden, "غير مصرح", null);

            if (currentUser.DepartmentId.HasValue)
            {
                return await _context.Departments
                    .Where(d => d.Id == currentUser.DepartmentId.Value && d.AgencyId == agencyId)
                    .Select(d => new { d.Id, d.Name, d.AgencyId })
                    .OrderBy(d => d.Name)
                    .ToListAsync(cancellationToken);
            }
        }

        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.AgencyId == agencyId)
            .Select(d => new { d.Id, d.Name, d.AgencyId })
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<object> CreateDepartmentAsync(UpsertDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeName(dto.Name) ?? throw BadRequest("اسم الإدارة مطلوب");
        if (!await _context.Agencies.AnyAsync(a => a.Id == dto.AgencyId, cancellationToken))
            throw BadRequest("الوكالة المحددة غير موجودة");
        if (await _context.Departments.AnyAsync(d => d.AgencyId == dto.AgencyId && d.Name.ToLower() == normalizedName.ToLower(), cancellationToken))
            throw BadRequest("اسم الإدارة مستخدم بالفعل داخل الوكالة المحددة");

        var department = new Department { Name = normalizedName, AgencyId = dto.AgencyId };
        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);
        return new { department.Id, department.Name, department.AgencyId };
    }

    public async Task<object> UpdateDepartmentAsync(int id, UpsertDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments.FindAsync(new object[] { id }, cancellationToken) ?? throw NotFound();
        var normalizedName = NormalizeName(dto.Name) ?? throw BadRequest("اسم الإدارة مطلوب");
        if (!await _context.Agencies.AnyAsync(a => a.Id == dto.AgencyId, cancellationToken))
            throw BadRequest("الوكالة المحددة غير موجودة");
        if (await _context.Departments.AnyAsync(d => d.AgencyId == dto.AgencyId && d.Name.ToLower() == normalizedName.ToLower() && d.Id != id, cancellationToken))
            throw BadRequest("اسم الإدارة مستخدم بالفعل داخل الوكالة المحددة");

        department.Name = normalizedName;
        department.AgencyId = dto.AgencyId;
        await _context.SaveChangesAsync(cancellationToken);
        return new { department.Id, department.Name, department.AgencyId };
    }

    public async Task DeleteDepartmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var hasChildren = await _context.Sections.AnyAsync(s => s.DepartmentId == id, cancellationToken)
            || await _context.Users.AnyAsync(u => u.DepartmentId == id, cancellationToken)
            || await _context.Items.AnyAsync(i => i.DepartmentId == id, cancellationToken);
        if (hasChildren)
            throw BadRequest("لا يمكن حذف الإدارة لوجود بيانات مرتبطة بها");

        var department = await _context.Departments.FindAsync(new object[] { id }, cancellationToken) ?? throw NotFound();
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<object> GetSectionsAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default)
    {
        var query = _context.Sections.AsNoTracking().AsQueryable();
        if (currentRole == "MANAGER")
        {
            var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
            if (currentUser?.SectionId != null)
                query = query.Where(s => s.Id == currentUser.SectionId.Value);
            else if (currentUser?.DepartmentId != null)
                query = query.Where(s => s.DepartmentId == currentUser.DepartmentId.Value);
            else
                query = query.Take(0);
        }

        return await query.Select(s => new { s.Id, s.Name, s.DepartmentId, DepartmentName = s.Department.Name }).OrderBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public async Task<object> GetSectionsByDepartmentAsync(int departmentId, int currentUserId, string currentRole, CancellationToken cancellationToken = default)
    {
        var currentUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);
        if (currentRole == "MANAGER" && currentUser != null)
        {
            if (currentUser.DepartmentId.HasValue && currentUser.DepartmentId != departmentId)
                throw new ApiException(StatusCodes.Status403Forbidden, "غير مصرح", null);
            if (currentUser.SectionId.HasValue)
            {
                return await _context.Sections
                    .Where(s => s.Id == currentUser.SectionId.Value && s.DepartmentId == departmentId)
                    .Select(s => new { s.Id, s.Name, s.DepartmentId })
                    .OrderBy(s => s.Name)
                    .ToListAsync(cancellationToken);
            }
        }

        return await _context.Sections
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId)
            .Select(s => new { s.Id, s.Name, s.DepartmentId })
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<object> CreateSectionAsync(UpsertSectionDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeName(dto.Name) ?? throw BadRequest("اسم الشعبة مطلوب");
        if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId, cancellationToken))
            throw BadRequest("الإدارة المحددة غير موجودة");
        if (await _context.Sections.AnyAsync(s => s.DepartmentId == dto.DepartmentId && s.Name.ToLower() == normalizedName.ToLower(), cancellationToken))
            throw BadRequest("اسم الشعبة مستخدم بالفعل داخل الإدارة المحددة");

        var section = new Section { Name = normalizedName, DepartmentId = dto.DepartmentId };
        _context.Sections.Add(section);
        await _context.SaveChangesAsync(cancellationToken);
        return new { section.Id, section.Name, section.DepartmentId };
    }

    public async Task<object> UpdateSectionAsync(int id, UpsertSectionDto dto, CancellationToken cancellationToken = default)
    {
        var section = await _context.Sections.FindAsync(new object[] { id }, cancellationToken) ?? throw NotFound();
        var normalizedName = NormalizeName(dto.Name) ?? throw BadRequest("اسم الشعبة مطلوب");
        if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId, cancellationToken))
            throw BadRequest("الإدارة المحددة غير موجودة");
        if (await _context.Sections.AnyAsync(s => s.DepartmentId == dto.DepartmentId && s.Name.ToLower() == normalizedName.ToLower() && s.Id != id, cancellationToken))
            throw BadRequest("اسم الشعبة مستخدم بالفعل داخل الإدارة المحددة");

        section.Name = normalizedName;
        section.DepartmentId = dto.DepartmentId;
        await _context.SaveChangesAsync(cancellationToken);
        return new { section.Id, section.Name, section.DepartmentId };
    }

    public async Task DeleteSectionAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(u => u.SectionId == id, cancellationToken))
            throw BadRequest("لا يمكن حذف الشعبة لوجود مستخدمين مرتبطين بها");

        var section = await _context.Sections.FindAsync(new object[] { id }, cancellationToken) ?? throw NotFound();
        _context.Sections.Remove(section);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string? NormalizeName(string? value) => string.IsNullOrWhiteSpace(value?.Trim()) ? null : value.Trim();
    private static ApiException BadRequest(string message) => new(StatusCodes.Status400BadRequest, message, new { message });
    private static ApiException NotFound() => new(StatusCodes.Status404NotFound, "العنصر غير موجود", null);
}
