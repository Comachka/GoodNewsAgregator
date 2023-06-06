using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace myProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlsAndShortDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Link",
                table: "NewsResources",
                newName: "RssFeedUrl");

            migrationBuilder.AddColumn<string>(
                name: "OriginUrl",
                table: "NewsResources",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "PositiveRaiting",
                table: "Articles",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ArticleSourceUrl",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginUrl",
                table: "NewsResources");

            migrationBuilder.DropColumn(
                name: "ArticleSourceUrl",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "RssFeedUrl",
                table: "NewsResources",
                newName: "Link");

            migrationBuilder.AlterColumn<int>(
                name: "PositiveRaiting",
                table: "Articles",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
