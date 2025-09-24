using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesWithProducts.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserCouponA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserCoupons",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserDto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_UserId1",
                table: "UserCoupons",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_UserDto_UserId1",
                table: "UserCoupons",
                column: "UserId1",
                principalTable: "UserDto",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_UserDto_UserId1",
                table: "UserCoupons");

            migrationBuilder.DropTable(
                name: "UserDto");

            migrationBuilder.DropIndex(
                name: "IX_UserCoupons_UserId1",
                table: "UserCoupons");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserCoupons");
        }
    }
}
