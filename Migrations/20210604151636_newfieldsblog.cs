using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NemesysZ2.Migrations
{
    public partial class newfieldsblog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateSpotted",
                table: "BlogPosts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSpotted",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "BlogPosts");
        }
    }
}
