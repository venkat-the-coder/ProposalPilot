using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProposalPilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBriefEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BriefId",
                table: "Proposals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Briefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RawContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnalyzedContent = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ProjectType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EstimatedBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Timeline = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KeyRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechnicalRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetAudience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AnalyzedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TokensUsed = table.Column<int>(type: "int", nullable: true),
                    AnalysisCost = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Briefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Briefs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_BriefId",
                table: "Proposals",
                column: "BriefId");

            migrationBuilder.CreateIndex(
                name: "IX_Briefs_CreatedAt",
                table: "Briefs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Briefs_Status",
                table: "Briefs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Briefs_UserId",
                table: "Briefs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Briefs_BriefId",
                table: "Proposals",
                column: "BriefId",
                principalTable: "Briefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Briefs_BriefId",
                table: "Proposals");

            migrationBuilder.DropTable(
                name: "Briefs");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_BriefId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "BriefId",
                table: "Proposals");
        }
    }
}
