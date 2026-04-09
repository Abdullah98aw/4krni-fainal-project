namespace Thakkirni.API.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string JobTitle { get; set; }

        public int? AgencyId { get; set; }
        public string AgencyName { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int? SectionId { get; set; }
        public string SectionName { get; set; }
    }
}
