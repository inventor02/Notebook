using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notebook.Api.Migrations
{
    public partial class NotesCanBePrivate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "public",
                table: "notes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "public",
                table: "notes");
        }
    }
}
