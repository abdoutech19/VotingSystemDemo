using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace EFDataAccessLibrary.Migrations
{
    public partial class AddSessionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowedToVerify",
                table: "Voters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "Voters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VotingSessionId",
                table: "Voters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VotingSessionId",
                table: "ConsensusNodes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VotingSessionId",
                table: "Candidates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<string>(maxLength: 50, nullable: false),
                    PrivateKey = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voters_VotingSessionId",
                table: "Voters",
                column: "VotingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsensusNodes_VotingSessionId",
                table: "ConsensusNodes",
                column: "VotingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_VotingSessionId",
                table: "Candidates",
                column: "VotingSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Sessions_VotingSessionId",
                table: "Candidates",
                column: "VotingSessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsensusNodes_Sessions_VotingSessionId",
                table: "ConsensusNodes",
                column: "VotingSessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voters_Sessions_VotingSessionId",
                table: "Voters",
                column: "VotingSessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Sessions_VotingSessionId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsensusNodes_Sessions_VotingSessionId",
                table: "ConsensusNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Voters_Sessions_VotingSessionId",
                table: "Voters");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Voters_VotingSessionId",
                table: "Voters");

            migrationBuilder.DropIndex(
                name: "IX_ConsensusNodes_VotingSessionId",
                table: "ConsensusNodes");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_VotingSessionId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "AllowedToVerify",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "VotingSessionId",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "VotingSessionId",
                table: "ConsensusNodes");

            migrationBuilder.DropColumn(
                name: "VotingSessionId",
                table: "Candidates");
        }
    }
}
