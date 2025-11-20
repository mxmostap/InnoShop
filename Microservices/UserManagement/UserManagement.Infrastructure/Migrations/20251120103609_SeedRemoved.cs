using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsActive", "PasswordHash", "Role", "UserName" },
                values: new object[] { 1, "mxmostapwork@gmail.com", true, "$2a$11$rLZeB6R2kS5Wq2qKkE5M5eMvJQY9W5ZQY5X5X5X5X5X5X5X5X5X5", "Admin", "admin" });
        }
    }
}
