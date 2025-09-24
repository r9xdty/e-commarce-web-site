using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesWithProducts.Migrations.CategoryAuthDb
{
    /// <inheritdoc />
    public partial class AddAdminUserAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9242dc24-7706-4e24-bf4a-e2f6373dbf3f", "9242dc24-7706-4e24-bf4a-e2f6373dbf3f", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "50a23dd6-bd3a-40ab-b842-67bfbc99852f", 0, "f76c4a18-85e7-4c48-8c3b-561b03e9dab5", "admin@thelist.com", true, false, null, "ADMIN@THELIST.COM", "ADMIN@THELIST.COM", "AQAAAAIAAYagAAAAELUUNQqEUNqErHzuFeYlgq3d2Y0JFcRU4i828HCSfwVAYIrIKZE2Pi8b21rwCt1B1g==", null, false, "60f09105-70fb-49a9-9621-b1e9f1888c40", false, "admin@thelist.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "9242dc24-7706-4e24-bf4a-e2f6373dbf3f", "50a23dd6-bd3a-40ab-b842-67bfbc99852f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "9242dc24-7706-4e24-bf4a-e2f6373dbf3f", "50a23dd6-bd3a-40ab-b842-67bfbc99852f" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9242dc24-7706-4e24-bf4a-e2f6373dbf3f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "50a23dd6-bd3a-40ab-b842-67bfbc99852f");
        }
    }
}
