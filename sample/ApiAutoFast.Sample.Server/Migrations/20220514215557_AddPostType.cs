using ApiAutoFast.Sample.Server.Database;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiAutoFast.Sample.Server.Migrations
{
    public partial class AddPostType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<EPostType>(
                name: "PostType",
                table: "Posts",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostType",
                table: "Posts");
        }
    }
}
