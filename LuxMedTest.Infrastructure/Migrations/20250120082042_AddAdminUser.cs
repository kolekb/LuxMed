using LuxMedTest.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxMedTest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var passwordHasher = new PasswordHasher();
            var hashedPassword = passwordHasher.HashPassword("admin123");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Username", "Password" },
                values: new object[] { 1, "admin", hashedPassword });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
