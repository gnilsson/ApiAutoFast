using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiAutoFast.Sample.Server.Migrations
{
    public partial class AddPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedDateTime_Id",
                table: "Posts",
                columns: new[] { "CreatedDateTime", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CreatedDateTime_Id",
                table: "Blogs",
                columns: new[] { "CreatedDateTime", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedDateTime_Id",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_CreatedDateTime_Id",
                table: "Blogs");
        }
    }
}
