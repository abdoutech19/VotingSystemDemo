using Microsoft.EntityFrameworkCore.Migrations;

namespace EFDataAccessLibrary.Migrations
{
    public partial class NullableVoterState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voters_States_VoterStateId",
                table: "Voters");

            migrationBuilder.AlterColumn<int>(
                name: "VoterStateId",
                table: "Voters",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Voters_States_VoterStateId",
                table: "Voters",
                column: "VoterStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Voters_States_VoterStateId",
                table: "Voters",
                column: "VoterStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
