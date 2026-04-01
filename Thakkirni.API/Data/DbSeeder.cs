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
                var company = new Company { Name = "الشركة الرئيسية" };
                context.Companies.Add(company);
                context.SaveChanges();

                var dept1 = new Department { Name = "إدارة تقنية المعلومات", CompanyId = company.Id };
                var dept2 = new Department { Name = "الموارد البشرية", CompanyId = company.Id };
                context.Departments.AddRange(dept1, dept2);
                context.SaveChanges();

                var user1 = new User { Name = "أحمد محمد العمري", Email = "ahmed@example.com", NationalId = "1234567890", Role = "ADMIN", DepartmentId = dept1.Id, Avatar = "" };
                var user2 = new User { Name = "سارة خالد الزهراني", Email = "sara@example.com", NationalId = "2345678901", Role = "USER", DepartmentId = dept1.Id, Avatar = "" };
                var user3 = new User { Name = "محمد عبدالله القحطاني", Email = "mohammed@example.com", NationalId = "3456789012", Role = "USER", DepartmentId = dept2.Id, Avatar = "" };
                context.Users.AddRange(user1, user2, user3);
                context.SaveChanges();
            }
        }
    }
}
