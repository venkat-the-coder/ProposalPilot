using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProposalPilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "Proposals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProposalTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultPricing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystemTemplate = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    WinRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstimatedTimeMinutes = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_TemplateId",
                table: "Proposals",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalTemplates_Category",
                table: "ProposalTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalTemplates_Composite",
                table: "ProposalTemplates",
                columns: new[] { "IsDeleted", "IsSystemTemplate", "UserId" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalTemplates_IsPublic",
                table: "ProposalTemplates",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalTemplates_IsSystemTemplate",
                table: "ProposalTemplates",
                column: "IsSystemTemplate");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalTemplates_UserId",
                table: "ProposalTemplates",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_ProposalTemplates_TemplateId",
                table: "Proposals",
                column: "TemplateId",
                principalTable: "ProposalTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_ProposalTemplates_TemplateId",
                table: "Proposals");

            migrationBuilder.DropTable(
                name: "ProposalTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_TemplateId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Proposals");
        }
    }
}
