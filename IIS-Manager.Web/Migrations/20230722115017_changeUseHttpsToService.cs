using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IIS_Manager.Web.Migrations
{
    public partial class changeUseHttpsToService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseHttps",
                table: "IisServer");

            migrationBuilder.AddColumn<string>(
                name: "Service",
                table: "IisServer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Service",
                table: "IisServer");

            migrationBuilder.AddColumn<bool>(
                name: "UseHttps",
                table: "IisServer",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
