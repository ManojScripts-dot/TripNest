using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripNest.Migrations
{
    /// <inheritdoc />
    public partial class MakeTourFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Agencies_AgencyId",
                table: "Tours");

            migrationBuilder.AlterColumn<int>(
                name: "AgencyId",
                table: "Tours",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Agencies_AgencyId",
                table: "Tours",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Agencies_AgencyId",
                table: "Tours");

            migrationBuilder.AlterColumn<int>(
                name: "AgencyId",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Agencies_AgencyId",
                table: "Tours",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
