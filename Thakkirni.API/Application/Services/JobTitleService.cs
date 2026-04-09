using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Application.Common.Exceptions;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.Data;
using Thakkirni.API.DTOs;
using Thakkirni.API.Models;

namespace Thakkirni.API.Application.Services;

public sealed class JobTitleService : IJobTitleService
{
    private readonly AppDbContext _context;

    public JobTitleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetJobTitlesAsync(CancellationToken cancellationToken = default)
        => await _context.JobTitles.AsNoTracking().OrderBy(j => j.Name).Select(j => new { j.Id, j.Name }).ToListAsync(cancellationToken);

    public async Task<object> CreateJobTitleAsync(UpsertJobTitleDto dto, CancellationToken cancellationToken = default)
    {
        var name = Normalize(dto.Name) ?? throw new ApiException(StatusCodes.Status400BadRequest, "اسم المسمى الوظيفي مطلوب", new { message = "اسم المسمى الوظيفي مطلوب" });
        if (await _context.JobTitles.AnyAsync(j => j.Name.ToLower() == name.ToLower(), cancellationToken))
            throw new ApiException(StatusCodes.Status409Conflict, "المسمى الوظيفي موجود بالفعل", new { message = "المسمى الوظيفي موجود بالفعل" });

        var jobTitle = new JobTitle { Name = name };
        _context.JobTitles.Add(jobTitle);
        await _context.SaveChangesAsync(cancellationToken);
        return new { jobTitle.Id, jobTitle.Name };
    }

    public async Task<object> UpdateJobTitleAsync(int id, UpsertJobTitleDto dto, CancellationToken cancellationToken = default)
    {
        var jobTitle = await _context.JobTitles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "غير موجود", null);
        jobTitle.Name = Normalize(dto.Name) ?? throw new ApiException(StatusCodes.Status400BadRequest, "اسم المسمى الوظيفي مطلوب", new { message = "اسم المسمى الوظيفي مطلوب" });
        await _context.SaveChangesAsync(cancellationToken);
        return new { jobTitle.Id, jobTitle.Name };
    }

    public async Task DeleteJobTitleAsync(int id, CancellationToken cancellationToken = default)
    {
        var jobTitle = await _context.JobTitles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "غير موجود", null);
        var inUse = await _context.Users.AnyAsync(u => u.JobTitle == jobTitle.Name, cancellationToken);
        if (inUse)
            throw new ApiException(StatusCodes.Status400BadRequest, "لا يمكن حذف المسمى الوظيفي لوجود مستخدمين مرتبطين به", new { message = "لا يمكن حذف المسمى الوظيفي لوجود مستخدمين مرتبطين به" });
        _context.JobTitles.Remove(jobTitle);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value?.Trim()) ? null : value.Trim();
}
