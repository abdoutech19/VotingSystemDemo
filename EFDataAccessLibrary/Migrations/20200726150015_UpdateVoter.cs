using Microsoft.EntityFrameworkCore.Migrations;

namespace EFDataAccessLibrary.Migrations
{
    public partial class UpdateVoter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voters_States_VoterStateId",
                table: "Voters");

            migrationBuilder.AlterColumn<int>(
                name: "VoterStateId",
                table: "Voters",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Voters_States_VoterStateId",
                table: "Voters",
                column: "VoterStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voters_States_VoterStateId",
                table: "Voters");

            migrationBuilder.AlterColumn<int>(
                name: "VoterStateId",
                table: "Voters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Voters_States_VoterStateId",
                table: "Voters",
                column: "VoterStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
