using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesWithProducts.Migrations.CategoryAuthDb
{
    /// <inheritdoc />
    public partial class UpdatingAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "50a23dd6-bd3a-40ab-b842-67bfbc99852f",
                columns: new[] { "ConcurrencyStamp", "IsDeleted", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2c39fbcd-7001-4d61-8861-919b9f4d8da7", false, "AQAAAAIAAYagAAAAEC8bVvZTCUIw0ENVdrm0LK9i0ObvXunb4g8yUTX2xV5Gj1YeIt2WXXLD7nbq9eMv7Q==", "afb1c038-4693-4b4b-bbdb-aab613fa840b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "50a23dd6-bd3a-40ab-b842-67bfbc99852f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e275c970-4502-4037-b134-6fdea1a039e5", "AQAAAAIAAYagAAAAEGtmmM8IKFqg3FlrYHYBPx7h+3EcLcD5NV7vsRWgYxlYV8ikCFMK0jg0tkQEIue1eA==", "50bbf964-46e3-401e-9ad3-011db42ad645" });
        }
    }
}
