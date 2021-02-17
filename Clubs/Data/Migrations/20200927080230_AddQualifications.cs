using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Clubs.Data.Migrations
{
    public partial class AddQualifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Qualifications",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Qualifications",
                table: "AspNetUsers");
        }
    }
}
