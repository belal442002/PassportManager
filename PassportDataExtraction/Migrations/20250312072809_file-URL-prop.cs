using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Intelligence_Task.Migrations
{
    /// <inheritdoc />
    public partial class fileURLprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fileUrl",
                table: "Passports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fileUrl",
                table: "Passports");
        }
    }
}
