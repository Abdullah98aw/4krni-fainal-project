using Thakkirni.API.DTOs;
using Thakkirni.API.Models;

namespace Thakkirni.API.Application.Mappers;

public static class ItemMapper
{
    public static ItemDto ToDto(Item item, bool hasUnread = false)
    {
        var memberDetails = item.Members?
            .Where(m => m.User != null)
            .GroupBy(m => m.UserId)
            .Select(g => ToUserDto(g.First().User!))
            .ToList() ?? new List<UserDto>();

        var assigneeDetails = item.Assignees?
            .Where(a => a.User != null)
            .GroupBy(a => a.UserId)
            .Select(g => ToUserDto(g.First().User!))
            .ToList() ?? new List<UserDto>();

        return new ItemDto
        {
            Id = item.Id,
            ItemNumber = item.ItemNumber ?? string.Empty,
            Type = item.Type ?? string.Empty,
            Title = item.Title ?? string.Empty,
            Description = item.Description ?? string.Empty,
            Importance = item.Importance ?? string.Empty,
            CommitteeType = item.CommitteeType,
            Status = item.ComputedStatus,
            CompletedDate = item.CompletedDate,
            DueDate = item.DueDate,
            CreatedById = item.CreatedById,
            CreatedByName = item.CreatedBy?.Name ?? string.Empty,
            DepartmentId = item.DepartmentId,
            DepartmentName = item.Department?.Name ?? string.Empty,
            AgencyId = item.Department?.AgencyId,
            AgencyName = item.Department?.Agency?.Name ?? string.Empty,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            MemberIds = item.Members?.Select(m => m.UserId).Distinct().ToList() ?? new(),
            AssigneeIds = item.Assignees?.Select(a => a.UserId).Distinct().ToList() ?? new(),
            Members = memberDetails,
            Assignees = assigneeDetails,
            HasUnreadUpdates = hasUnread
        };
    }

    private static UserDto ToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            NationalId = user.NationalId ?? string.Empty,
            Role = user.Role ?? string.Empty,
            Avatar = user.Avatar ?? string.Empty,
            JobTitle = user.JobTitle ?? string.Empty,
            AgencyId = user.AgencyId,
            AgencyName = user.Agency?.Name ?? string.Empty,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department?.Name ?? string.Empty,
            SectionId = user.SectionId,
            SectionName = user.Section?.Name ?? string.Empty
        };
    }
}
