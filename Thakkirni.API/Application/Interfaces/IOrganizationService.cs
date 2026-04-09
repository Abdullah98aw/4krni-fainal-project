using Thakkirni.API.DTOs;

namespace Thakkirni.API.Application.Interfaces;

public interface IOrganizationService
{
    Task<object> GetAgenciesAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<object> CreateAgencyAsync(UpsertAgencyDto dto, CancellationToken cancellationToken = default);
    Task<object> UpdateAgencyAsync(int id, UpsertAgencyDto dto, CancellationToken cancellationToken = default);
    Task DeleteAgencyAsync(int id, CancellationToken cancellationToken = default);
    Task<object> GetDepartmentsAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<object> GetDepartmentsByAgencyAsync(int agencyId, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<object> CreateDepartmentAsync(UpsertDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<object> UpdateDepartmentAsync(int id, UpsertDepartmentDto dto, CancellationToken cancellationToken = default);
    Task DeleteDepartmentAsync(int id, CancellationToken cancellationToken = default);
    Task<object> GetSectionsAsync(int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<object> GetSectionsByDepartmentAsync(int departmentId, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<object> CreateSectionAsync(UpsertSectionDto dto, CancellationToken cancellationToken = default);
    Task<object> UpdateSectionAsync(int id, UpsertSectionDto dto, CancellationToken cancellationToken = default);
    Task DeleteSectionAsync(int id, CancellationToken cancellationToken = default);
}
