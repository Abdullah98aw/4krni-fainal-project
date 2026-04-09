namespace Thakkirni.API.DTOs;

public sealed class UpsertAgencyDto
{
    public string Name { get; set; } = string.Empty;
}

public sealed class UpsertDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public int AgencyId { get; set; }
}

public sealed class UpsertSectionDto
{
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}
