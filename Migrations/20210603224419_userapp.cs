using Microsoft.EntityFrameworkCore.Migrations;

namespace NemesysZ2.Migrations
{
    public partial class userapp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserChangeLimit",
                table: "AspNetUsers",
                newName: "UsernameChangeLimit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsernameChangeLimit",
                table: "AspNetUsers",
                newName: "UserChangeLimit");
        }
    }
}
