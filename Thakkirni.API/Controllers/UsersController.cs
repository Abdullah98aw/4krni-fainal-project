using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Data;
using Thakkirni.API.DTOs;
using Thakkirni.API.Models;

namespace Thakkirni.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.Department)
                .Include(u => u.Section)
                .Select(u => new UserDto
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
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUpdateUserDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                NationalId = dto.NationalId ?? "",
                Role = dto.Role ?? "USER",
                Avatar = dto.Avatar ?? "",
                JobTitle = dto.JobTitle ?? "",
                AgencyId = dto.AgencyId > 0 ? dto.AgencyId : null,
                DepartmentId = dto.DepartmentId > 0 ? dto.DepartmentId : null,
                SectionId = dto.SectionId > 0 ? dto.SectionId : null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(user).Reference(u => u.Agency).LoadAsync();
            await _context.Entry(user).Reference(u => u.Department).LoadAsync();
            await _context.Entry(user).Reference(u => u.Section).LoadAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, MapToDto(user));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUpdateUserDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.Department)
                .Include(u => u.Section)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Name)) user.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.NationalId)) user.NationalId = dto.NationalId;
            if (!string.IsNullOrEmpty(dto.Role)) user.Role = dto.Role;
            if (dto.JobTitle != null) user.JobTitle = dto.JobTitle;

            // Allow explicit null to clear, or positive value to set
            if (dto.AgencyId.HasValue)
                user.AgencyId = dto.AgencyId > 0 ? dto.AgencyId : null;

            if (dto.DepartmentId.HasValue)
                user.DepartmentId = dto.DepartmentId > 0 ? dto.DepartmentId : null;

            if (dto.SectionId.HasValue)
                user.SectionId = dto.SectionId > 0 ? dto.SectionId : null;

            await _context.SaveChangesAsync();

            // Reload navigation properties after save
            await _context.Entry(user).Reference(u => u.Agency).LoadAsync();
            await _context.Entry(user).Reference(u => u.Department).LoadAsync();
            await _context.Entry(user).Reference(u => u.Section).LoadAsync();

            return Ok(MapToDto(user));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

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
            AgencyName = user.Agency?.Name,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department?.Name,
            SectionId = user.SectionId,
            SectionName = user.Section?.Name
        };
    }
}
