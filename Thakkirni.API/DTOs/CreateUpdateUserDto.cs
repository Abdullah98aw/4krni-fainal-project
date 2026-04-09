using System.ComponentModel.DataAnnotations;

namespace Thakkirni.API.DTOs
{
    public class CreateUpdateUserDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [MaxLength(100, ErrorMessage = "البريد الإلكتروني لا يتجاوز 100 حرف")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "رقم الهوية مطلوب")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "رقم الهوية يجب أن يكون 10 أرقام")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "رقم الهوية يجب أن يتكون من 10 أرقام فقط")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "الدور مطلوب")]
        [MaxLength(20, ErrorMessage = "الدور لا يتجاوز 20 حرفاً")]
        public string Role { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "المسمى الوظيفي لا يتجاوز 100 حرف")]
        public string? JobTitle { get; set; }

        [MinLength(6, ErrorMessage = "كلمة المرور يجب ألا تقل عن 6 أحرف")]
        public string? Password { get; set; }

        public string? Avatar { get; set; }

        public int? AgencyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
    }
}
