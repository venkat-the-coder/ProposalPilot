using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProposalPilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailTrackingAndFollowUps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SendGridMessageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstOpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastOpenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastClickedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BouncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpamReportedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnsubscribedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenCount = table.Column<int>(type: "int", nullable: false),
                    ClickCount = table.Column<int>(type: "int", nullable: false),
                    UniqueClickCount = table.Column<int>(type: "int", nullable: false),
                    BounceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BounceReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClickedLinksJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalizedMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TriggerReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DaysSinceLastContact = table.Column<int>(type: "int", nullable: false),
                    EmailLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowUps_EmailLogs_EmailLogId",
                        column: x => x.EmailLogId,
                        principalTable: "EmailLogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FollowUps_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_ProposalId",
                table: "EmailLogs",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUps_EmailLogId",
                table: "FollowUps",
                column: "EmailLogId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUps_ProposalId",
                table: "FollowUps",
                column: "ProposalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FollowUps");

            migrationBuilder.DropTable(
                name: "EmailLogs");
        }
    }
}
