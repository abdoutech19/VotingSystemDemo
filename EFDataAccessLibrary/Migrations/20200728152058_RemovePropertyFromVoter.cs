using Microsoft.EntityFrameworkCore.Migrations;

namespace EFDataAccessLibrary.Migrations
{
    public partial class RemovePropertyFromVoter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedToVerify",
                table: "Voters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "AllowedToVerify",
                table: "Voters",
                type: "bit",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
