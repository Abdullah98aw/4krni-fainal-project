using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly ICurrentUserService _currentUserService;

    public OrganizationsController(IOrganizationService organizationService, ICurrentUserService currentUserService)
    {
        _organizationService = organizationService;
        _currentUserService = currentUserService;
    }

    [HttpGet("agencies")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public async Task<IActionResult> GetAgencies(CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _organizationService.GetAgenciesAsync(currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpPost("agencies")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateAgency([FromBody] UpsertAgencyDto dto, CancellationToken cancellationToken)
        => Ok(await _organizationService.CreateAgencyAsync(dto, cancellationToken));

    [HttpPut("agencies/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateAgency(int id, [FromBody] UpsertAgencyDto dto, CancellationToken cancellationToken)
        => Ok(await _organizationService.UpdateAgencyAsync(id, dto, cancellationToken));

    [HttpDelete("agencies/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteAgency(int id, CancellationToken cancellationToken)
    {
        await _organizationService.DeleteAgencyAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("departments")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _organizationService.GetDepartmentsAsync(currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpGet("agencies/{agencyId}/departments")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public async Task<IActionResult> GetDepartmentsByAgency(int agencyId, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _organizationService.GetDepartmentsByAgencyAsync(agencyId, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpPost("departments")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateDepartment([FromBody] UpsertDepartmentDto dto, CancellationToken cancellationToken)
        => Ok(await _organizationService.CreateDepartmentAsync(dto, cancellationToken));

    [HttpPut("departments/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpsertDepartmentDto dto, CancellationToken cancellationToken)
        => Ok(await _organizationService.UpdateDepartmentAsync(id, dto, cancellationToken));

    [HttpDelete("departments/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteDepartment(int id, CancellationToken cancellationToken)
    {
        await _organizationService.DeleteDepartmentAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("sections")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public async Task<IActionResult> GetSections(CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _organizationService.GetSectionsAsync(currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpGet("departments/{departmentId}/sections")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public async Task<IActionResult> GetSectionsByDepartment(int departmentId, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _organizationService.GetSectionsByDepartmentAsync(departmentId, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpPost("sections")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateSection([FromBody] UpsertSectionDto dto, CancellationToken cancellationToken)
        => Ok(await _organizationService.CreateSectionAsync(dto, cancellationToken));

    [HttpPut("sections/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateSection(int id, [FromBody] UpsertSectionDto dto, CancellationToken cancellationToken)
        => Ok(await _organizationService.UpdateSectionAsync(id, dto, cancellationToken));

    [HttpDelete("sections/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteSection(int id, CancellationToken cancellationToken)
    {
        await _organizationService.DeleteSectionAsync(id, cancellationToken);
        return NoContent();
    }
}
