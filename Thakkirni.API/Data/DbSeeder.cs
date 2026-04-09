using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Models;
<<<<<<< HEAD
using Thakkirni.API.Services;
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62

namespace Thakkirni.API.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();
<<<<<<< HEAD
            EnsureSchema(context);

            if (!context.JobTitles.Any())
            {
                context.JobTitles.AddRange(
                    new JobTitle { Name = "مدير تقنية المعلومات" },
                    new JobTitle { Name = "مهندسة دعم فني" },
                    new JobTitle { Name = "أخصائي موارد بشرية" },
                    new JobTitle { Name = "مدير إدارة" },
                    new JobTitle { Name = "مدير وكالة" },
                    new JobTitle { Name = "مدير شعبة" }
                );
                context.SaveChanges();
            }
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62

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
<<<<<<< HEAD
                    PasswordHash = PasswordHasherService.HashPassword("Admin@123"),
                    Avatar = string.Empty
=======
                    Avatar = ""
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
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
<<<<<<< HEAD
                    PasswordHash = PasswordHasherService.HashPassword("User@123"),
                    Avatar = string.Empty
=======
                    Avatar = ""
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
                };
                var user3 = new User
                {
                    Name = "محمد عبدالله القحطاني",
                    Email = "mohammed@example.com",
                    NationalId = "3456789012",
<<<<<<< HEAD
                    Role = "MANAGER",
                    AgencyId = agency.Id,
                    DepartmentId = dept2.Id,
                    SectionId = section3.Id,
                    JobTitle = "مدير إدارة",
                    PasswordHash = PasswordHasherService.HashPassword("Manager@123"),
                    Avatar = string.Empty
=======
                    Role = "USER",
                    AgencyId = agency.Id,
                    DepartmentId = dept2.Id,
                    SectionId = section3.Id,
                    JobTitle = "أخصائي موارد بشرية",
                    Avatar = ""
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
                };
                context.Users.AddRange(user1, user2, user3);
                context.SaveChanges();
            }
<<<<<<< HEAD
            else
            {
                var updated = false;
                foreach (var user in context.Users.Where(u => string.IsNullOrWhiteSpace(u.PasswordHash)))
                {
                    var fallbackPassword = user.Role == "ADMIN"
                        ? "Admin@123"
                        : user.Role == "MANAGER"
                            ? "Manager@123"
                            : "User@123";

                    user.PasswordHash = PasswordHasherService.HashPassword(fallbackPassword);
                    updated = true;
                }

                if (updated)
                    context.SaveChanges();
            }
        }

        private static void EnsureSchema(AppDbContext context)
        {
            if (context.Database.IsSqlServer())
            {
                context.Database.ExecuteSqlRaw(@"
IF COL_LENGTH('Users', 'PasswordHash') IS NULL
    ALTER TABLE Users ADD PasswordHash NVARCHAR(512) NULL;
");
            }
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
        }
    }
}
