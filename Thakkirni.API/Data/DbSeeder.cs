using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Models;

namespace Thakkirni.API.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var agency = new Agency { Name = "الجهة الرئيسية" };
                context.Agencies.Add(agency);
                context.SaveChanges();

                var dept1 = new Department { Name = "إدارة تقنية المعلومات", AgencyId = agency.Id };
                var dept2 = new Department { Name = "الموارد البشرية", AgencyId = agency.Id };
                context.Departments.AddRange(dept1, dept2);
                context.SaveChanges();

                var section1 = new Section { Name = "قسم التطوير", DepartmentId = dept1.Id };
                var section2 = new Section { Name = "قسم الدعم الفني", DepartmentId = dept1.Id };
                var section3 = new Section { Name = "قسم التوظيف", DepartmentId = dept2.Id };
                context.Sections.AddRange(section1, section2, section3);
                context.SaveChanges();

                var user1 = new User
                {
                    Name = "أحمد محمد العمري",
                    Email = "ahmed@example.com",
                    NationalId = "1234567890",
                    Role = "ADMIN",
                    AgencyId = agency.Id,
                    DepartmentId = dept1.Id,
                    SectionId = section1.Id,
                    JobTitle = "مدير تقنية المعلومات",
                    Avatar = ""
                };
                var user2 = new User
                {
                    Name = "سارة خالد الزهراني",
                    Email = "sara@example.com",
                    NationalId = "2345678901",
                    Role = "USER",
                    AgencyId = agency.Id,
                    DepartmentId = dept1.Id,
                    SectionId = section2.Id,
                    JobTitle = "مهندسة دعم فني",
                    Avatar = ""
                };
                var user3 = new User
                {
                    Name = "محمد عبدالله القحطاني",
                    Email = "mohammed@example.com",
                    NationalId = "3456789012",
                    Role = "USER",
                    AgencyId = agency.Id,
                    DepartmentId = dept2.Id,
                    SectionId = section3.Id,
                    JobTitle = "أخصائي موارد بشرية",
                    Avatar = ""
                };
                context.Users.AddRange(user1, user2, user3);
                context.SaveChanges();
            }
        }
    }
}
