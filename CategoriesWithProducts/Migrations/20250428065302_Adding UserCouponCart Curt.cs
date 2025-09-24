using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesWithProducts.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserCouponCartCurt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "UserCoupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserCoupons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "UserCoupons");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserCoupons");
        }
    }
}
