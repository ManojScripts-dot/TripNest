using Microsoft.EntityFrameworkCore.Migrations;

namespace TripNest.Migrations
{
    public partial class UpdateTourImagePaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE Tours
                  SET ImagePath = CONCAT('/', ImagePath)
                  WHERE ImagePath IS NOT NULL AND ImagePath NOT LIKE '/%';"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE Tours
                  SET ImagePath = SUBSTRING(ImagePath, 2)
                  WHERE ImagePath IS NOT NULL AND ImagePath LIKE '/%';"
            );
        }
    }
}