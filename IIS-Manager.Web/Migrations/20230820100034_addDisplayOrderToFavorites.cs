using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IIS_Manager.Web.Migrations
{
    public partial class addDisplayOrderToFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Favorite",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Favorite");
        }
    }
}
