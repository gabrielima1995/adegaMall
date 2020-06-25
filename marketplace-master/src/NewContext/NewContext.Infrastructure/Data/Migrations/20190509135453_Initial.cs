using Microsoft.EntityFrameworkCore.Migrations;

namespace NewContext.Infrastructure.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "nc");

            migrationBuilder.CreateTable(
                name: "PlatformLanguage",
                schema: "nc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 0),
                    ActiveLanguages = table.Column<string>(maxLength: 100, nullable: false),
                    DefaultLanguage = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformLanguage", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformLanguage",
                schema: "nc");
        }
    }
}
