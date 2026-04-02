using System.ComponentModel.DataAnnotations;

namespace Thakkirni.API.DTOs
{
    public class CreateUpdateUserDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "البريد الإلكتروني لا يتجاوز 100 حرف")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "رقم الهوية مطلوب")]
        [MaxLength(20, ErrorMessage = "رقم الهوية لا يتجاوز 20 حرفاً")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "الدور مطلوب")]
        [MaxLength(20, ErrorMessage = "الدور لا يتجاوز 20 حرفاً")]
        public string Role { get; set; }

        [MaxLength(100, ErrorMessage = "المسمى الوظيفي لا يتجاوز 100 حرف")]
        public string? JobTitle { get; set; }

        public string? Avatar { get; set; }

        public int? AgencyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
    }
}
