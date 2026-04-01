namespace Thakkirni.API.DTOs
{
    public class CreateUpdateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string JobTitle { get; set; }
        public int? AgencyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
    }
}
