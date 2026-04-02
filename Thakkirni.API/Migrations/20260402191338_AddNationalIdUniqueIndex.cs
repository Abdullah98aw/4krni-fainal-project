using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thakkirni.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNationalIdUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_NationalId",
                table: "Users",
                column: "NationalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_NationalId",
                table: "Users");
        }
    }
}
