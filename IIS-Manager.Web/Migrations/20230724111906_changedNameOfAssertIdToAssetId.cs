using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IIS_Manager.Web.Migrations
{
    public partial class changedNameOfAssertIdToAssetId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssertId",
                table: "Favorite",
                newName: "AssetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "Favorite",
                newName: "AssertId");
        }
    }
}
