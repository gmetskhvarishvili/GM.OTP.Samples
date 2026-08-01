using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.OTP.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inbox_messages");

            migrationBuilder.EnsureSchema(
                name: "application");

            migrationBuilder.EnsureSchema(
                name: "outbox_messages");

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "inbox_messages",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_messages", x => new { x.EventId, x.ConsumerName });
                });

            migrationBuilder.CreateTable(
                name: "OtpChallenges",
                schema: "application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Destination = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CodeHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Purpose = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailedAttempts = table.Column<int>(type: "integer", nullable: false),
                    MaxAttempts = table.Column<int>(type: "integer", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IsInvalidated = table.Column<bool>(type: "boolean", nullable: false),
                    UsedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InvalidatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "outbox_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_inbox_messages_processed_at",
                schema: "inbox_messages",
                table: "inbox_messages",
                column: "ProcessedAtUtc");

            migrationBuilder.CreateIndex(
                name: "ix_inbox_messages_user_id",
                schema: "inbox_messages",
                table: "inbox_messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_Subject_Purpose",
                schema: "application",
                table: "OtpChallenges",
                columns: new[] { "Subject", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_at",
                schema: "outbox_messages",
                table: "outbox_messages",
                column: "ProcessedAtUtc");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_user_id",
                schema: "outbox_messages",
                table: "outbox_messages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "inbox_messages");

            migrationBuilder.DropTable(
                name: "OtpChallenges",
                schema: "application");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "outbox_messages");
        }
    }
}
