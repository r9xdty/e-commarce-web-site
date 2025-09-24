using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CategoriesWithProducts.Migrations.CategoryAuthDb
{
    /// <inheritdoc />
    public partial class AddAdminUserAllRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "2411b3be-5a24-44bc-899c-be0708413bde", "50a23dd6-bd3a-40ab-b842-67bfbc99852f" },
                    { "c9139e79-84a5-4432-b07a-0fa1108816d7", "50a23dd6-bd3a-40ab-b842-67bfbc99852f" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "50a23dd6-bd3a-40ab-b842-67bfbc99852f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e275c970-4502-4037-b134-6fdea1a039e5", "AQAAAAIAAYagAAAAEGtmmM8IKFqg3FlrYHYBPx7h+3EcLcD5NV7vsRWgYxlYV8ikCFMK0jg0tkQEIue1eA==", "50bbf964-46e3-401e-9ad3-011db42ad645" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2411b3be-5a24-44bc-899c-be0708413bde", "50a23dd6-bd3a-40ab-b842-67bfbc99852f" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c9139e79-84a5-4432-b07a-0fa1108816d7", "50a23dd6-bd3a-40ab-b842-67bfbc99852f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "50a23dd6-bd3a-40ab-b842-67bfbc99852f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f76c4a18-85e7-4c48-8c3b-561b03e9dab5", "AQAAAAIAAYagAAAAELUUNQqEUNqErHzuFeYlgq3d2Y0JFcRU4i828HCSfwVAYIrIKZE2Pi8b21rwCt1B1g==", "60f09105-70fb-49a9-9621-b1e9f1888c40" });
        }
    }
}
