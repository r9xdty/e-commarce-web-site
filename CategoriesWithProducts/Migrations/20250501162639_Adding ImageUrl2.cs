using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesWithProducts.Migrations
{
    /// <inheritdoc />
    public partial class AddingImageUrl2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductImageUrl",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImageUrl",
                table: "OrderItems");
        }
    }
}
