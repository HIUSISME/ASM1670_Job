using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM1670_Job.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatusApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Application",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Application");
        }
    }
}
