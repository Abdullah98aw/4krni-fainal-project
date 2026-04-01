using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Data;

namespace Thakkirni.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrganizationsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/organizations/agencies
        /// Returns all agencies.
        /// </summary>
        [HttpGet("agencies")]
        public async Task<IActionResult> GetAgencies()
        {
            var agencies = await _context.Agencies
                .Select(a => new { a.Id, a.Name })
                .OrderBy(a => a.Name)
                .ToListAsync();

            return Ok(agencies);
        }

        /// <summary>
        /// GET /api/organizations/agencies/{agencyId}/departments
        /// Returns departments belonging to the specified agency.
        /// </summary>
        [HttpGet("agencies/{agencyId}/departments")]
        public async Task<IActionResult> GetDepartmentsByAgency(int agencyId)
        {
            var departments = await _context.Departments
                .Where(d => d.AgencyId == agencyId)
                .Select(d => new { d.Id, d.Name, d.AgencyId })
                .OrderBy(d => d.Name)
                .ToListAsync();

            return Ok(departments);
        }

        /// <summary>
        /// GET /api/organizations/departments/{departmentId}/sections
        /// Returns sections belonging to the specified department.
        /// </summary>
        [HttpGet("departments/{departmentId}/sections")]
        public async Task<IActionResult> GetSectionsByDepartment(int departmentId)
        {
            var sections = await _context.Sections
                .Where(s => s.DepartmentId == departmentId)
                .Select(s => new { s.Id, s.Name, s.DepartmentId })
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(sections);
        }
    }
}
