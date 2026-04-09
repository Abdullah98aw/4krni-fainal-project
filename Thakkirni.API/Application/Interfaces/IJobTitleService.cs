using Thakkirni.API.DTOs;

namespace Thakkirni.API.Application.Interfaces;

public interface IJobTitleService
{
    Task<object> GetJobTitlesAsync(CancellationToken cancellationToken = default);
    Task<object> CreateJobTitleAsync(UpsertJobTitleDto dto, CancellationToken cancellationToken = default);
    Task<object> UpdateJobTitleAsync(int id, UpsertJobTitleDto dto, CancellationToken cancellationToken = default);
    Task DeleteJobTitleAsync(int id, CancellationToken cancellationToken = default);
}
