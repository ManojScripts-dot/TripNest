using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripNest.Migrations
{
    /// <inheritdoc />
    public partial class RenameImageUrlToImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Tours",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Tours",
                newName: "ImageUrl");
        }
    }
}
