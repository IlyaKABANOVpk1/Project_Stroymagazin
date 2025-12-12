using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Stroymagazin.Migrations
{
    /// <inheritdoc />
    public partial class testkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$EH4Z5.jaQCIYk.F6EsUk3.F9FHyacfp4t19LtWWDob.f8Y9fmYFru");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$W/JscyavZkSGX00YwtlrUuVgTKRoEnVnhIWtNUSlRaVsxNskQLMDm");
        }
    }
}
