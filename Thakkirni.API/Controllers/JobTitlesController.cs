using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobTitlesController : ControllerBase
{
    private readonly IJobTitleService _jobTitleService;

    public JobTitlesController(IJobTitleService jobTitleService)
    {
        _jobTitleService = jobTitleService;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetJobTitles(CancellationToken cancellationToken)
        => Ok(await _jobTitleService.GetJobTitlesAsync(cancellationToken));

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateJobTitle([FromBody] UpsertJobTitleDto dto, CancellationToken cancellationToken)
        => Ok(await _jobTitleService.CreateJobTitleAsync(dto, cancellationToken));

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateJobTitle(int id, [FromBody] UpsertJobTitleDto dto, CancellationToken cancellationToken)
        => Ok(await _jobTitleService.UpdateJobTitleAsync(id, dto, cancellationToken));

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteJobTitle(int id, CancellationToken cancellationToken)
    {
        await _jobTitleService.DeleteJobTitleAsync(id, cancellationToken);
        return NoContent();
    }
}
