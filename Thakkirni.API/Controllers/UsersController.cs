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

        // ─────────────────────────────────────────────
        // GET /api/users
        // ─────────────────────────────────────────────
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

        // ─────────────────────────────────────────────
        // POST /api/users
        // ─────────────────────────────────────────────
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUpdateUserDto dto)
        {
            // 1. ModelState validation (Required / MaxLength from DTO annotations)
            if (!ModelState.IsValid)
                return BadRequest(BuildValidationErrors(ModelState));

            // 2. Role must be a known value
            if (dto.Role != "ADMIN" && dto.Role != "USER")
                return BadRequest(new { errors = new[] { "الدور يجب أن يكون ADMIN أو USER" } });

            // 3. JobTitle must be one of the predefined values (if provided)
            if (!string.IsNullOrWhiteSpace(dto.JobTitle) && !AllowedJobTitles.Contains(dto.JobTitle))
                return BadRequest(new { errors = new[] { $"المسمى الوظيفي غير صالح. القيم المسموح بها: {string.Join("، ", AllowedJobTitles)}" } });

            // 4. Duplicate NationalId check
            var nationalIdExists = await _context.Users
                .AnyAsync(u => u.NationalId == dto.NationalId);
            if (nationalIdExists)
                return BadRequest(new { errors = new[] { "رقم الهوية مستخدم بالفعل" } });

            // 5. Organization hierarchy validation
            var orgError = await ValidateOrgHierarchy(dto.AgencyId, dto.DepartmentId, dto.SectionId);
            if (orgError != null)
                return BadRequest(new { errors = new[] { orgError } });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email ?? "",
                NationalId = dto.NationalId,
                Role = dto.Role,
                Avatar = dto.Avatar ?? "",
                JobTitle = dto.JobTitle ?? "",
                AgencyId = dto.AgencyId > 0 ? dto.AgencyId : null,
                DepartmentId = dto.DepartmentId > 0 ? dto.DepartmentId : null,
                SectionId = dto.SectionId > 0 ? dto.SectionId : null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Agency).LoadAsync();
            await _context.Entry(user).Reference(u => u.Department).LoadAsync();
            await _context.Entry(user).Reference(u => u.Section).LoadAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, MapToDto(user));
        }

        // ─────────────────────────────────────────────
        // PUT /api/users/{id}
        // ─────────────────────────────────────────────
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUpdateUserDto dto)
        {
            // 1. ModelState validation
            if (!ModelState.IsValid)
                return BadRequest(BuildValidationErrors(ModelState));

            var user = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.Department)
                .Include(u => u.Section)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound(new { errors = new[] { "المستخدم غير موجود" } });

            // 2. Role must be a known value
            if (dto.Role != "ADMIN" && dto.Role != "USER")
                return BadRequest(new { errors = new[] { "الدور يجب أن يكون ADMIN أو USER" } });

            // 3. JobTitle must be one of the predefined values (if provided)
            if (!string.IsNullOrWhiteSpace(dto.JobTitle) && !AllowedJobTitles.Contains(dto.JobTitle))
                return BadRequest(new { errors = new[] { $"المسمى الوظيفي غير صالح. القيم المسموح بها: {string.Join("، ", AllowedJobTitles)}" } });

            // 4. Duplicate NationalId check (exclude current user)
            var nationalIdExists = await _context.Users
                .AnyAsync(u => u.NationalId == dto.NationalId && u.Id != id);
            if (nationalIdExists)
                return BadRequest(new { errors = new[] { "رقم الهوية مستخدم بالفعل" } });

            // Resolve the final org IDs (use dto values; null/0 means clear)
            int? finalAgencyId = dto.AgencyId > 0 ? dto.AgencyId : null;
            int? finalDepartmentId = dto.DepartmentId > 0 ? dto.DepartmentId : null;
            int? finalSectionId = dto.SectionId > 0 ? dto.SectionId : null;

            // 4. Organization hierarchy validation
            var orgError = await ValidateOrgHierarchy(finalAgencyId, finalDepartmentId, finalSectionId);
            if (orgError != null)
                return BadRequest(new { errors = new[] { orgError } });

            // Apply updates
            user.Name = dto.Name;
            user.Email = dto.Email ?? user.Email;
            user.NationalId = dto.NationalId;
            user.Role = dto.Role;
            user.JobTitle = dto.JobTitle ?? "";
            user.AgencyId = finalAgencyId;
            user.DepartmentId = finalDepartmentId;
            user.SectionId = finalSectionId;

            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Agency).LoadAsync();
            await _context.Entry(user).Reference(u => u.Department).LoadAsync();
            await _context.Entry(user).Reference(u => u.Section).LoadAsync();

            return Ok(MapToDto(user));
        }

        // ─────────────────────────────────────────────
        // DELETE /api/users/{id}
        // ─────────────────────────────────────────────
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { errors = new[] { "المستخدم غير موجود" } });
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ─────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────

        /// <summary>
        /// The allowed predefined job title values.
        /// </summary>
        private static readonly HashSet<string> AllowedJobTitles = new()
        {
            "مدير وكالة",
            "مدير إدارة",
            "مدير شعبة",
            "موظف"
        };

        private async Task<string?> ValidateOrgHierarchy(int? agencyId, int? departmentId, int? sectionId)
        {
            // Section requires Department
            if (sectionId.HasValue && sectionId > 0 && (!departmentId.HasValue || departmentId <= 0))
                return "لا يمكن تحديد القسم بدون تحديد الإدارة أولاً";

            // Department requires Agency
            if (departmentId.HasValue && departmentId > 0 && (!agencyId.HasValue || agencyId <= 0))
                return "لا يمكن تحديد الإدارة بدون تحديد الجهة أولاً";

            // Agency must exist
            if (agencyId.HasValue && agencyId > 0)
            {
                var agencyExists = await _context.Agencies.AnyAsync(a => a.Id == agencyId);
                if (!agencyExists)
                    return "الجهة المحددة غير موجودة";
            }

            // Department must exist and belong to the specified Agency
            if (departmentId.HasValue && departmentId > 0)
            {
                var dept = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == departmentId);
                if (dept == null)
                    return "الإدارة المحددة غير موجودة";
                if (agencyId.HasValue && dept.AgencyId != agencyId)
                    return "الإدارة المحددة لا تنتمي إلى الجهة المختارة";
            }

            // Section must exist and belong to the specified Department
            if (sectionId.HasValue && sectionId > 0)
            {
                var section = await _context.Sections
                    .FirstOrDefaultAsync(s => s.Id == sectionId);
                if (section == null)
                    return "القسم المحدد غير موجود";
                if (departmentId.HasValue && section.DepartmentId != departmentId)
                    return "القسم المحدد لا ينتمي إلى الإدارة المختارة";
            }

            return null;
        }

        /// <summary>
        /// Builds a structured error response from ModelState validation failures.
        /// </summary>
        private static object BuildValidationErrors(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            var errors = modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            return new { errors };
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
