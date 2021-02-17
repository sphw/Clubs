using Microsoft.EntityFrameworkCore.Migrations;

namespace Clubs.Data.Migrations
{
    public partial class AddRequiredQualifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequiredQualifications",
                table: "Trip",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredQualifications",
                table: "Trip");
        }
    }
}
