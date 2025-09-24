using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesWithProducts.Migrations
{
    /// <inheritdoc />
    public partial class AddedTypeOfCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPercent",
                table: "Coupons",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPercent",
                table: "Coupons");
        }
    }
}
