using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.OTP.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "domainEvents");

            migrationBuilder.EnsureSchema(
                name: "inbox_messages");

            migrationBuilder.EnsureSchema(
                name: "application");

            migrationBuilder.EnsureSchema(
                name: "outbox_messages");

            migrationBuilder.CreateTable(
                name: "DomainEvents",
                schema: "domainEvents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregate_type = table.Column<string>(type: "text", nullable: true),
                    aggregate_id = table.Column<string>(type: "text", nullable: true),
                    event_type = table.Column<string>(type: "text", nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    occurred_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    client_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    channel_id = table.Column<string>(type: "text", nullable: true),
                    culture = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "text", nullable: true),
                    correlation_id = table.Column<string>(type: "text", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    idempotency_key = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_domain_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "inbox_messages",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consumer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    event_type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    received_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_messages", x => new { x.event_id, x.consumer_name });
                });

            migrationBuilder.CreateTable(
                name: "OtpChallenges",
                schema: "application",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_hidden = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    destination = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    code_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    salt = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    purpose = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false),
                    max_attempts = table.Column<int>(type: "integer", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    is_invalidated = table.Column<bool>(type: "boolean", nullable: false),
                    used_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    invalidated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_otp_challenges", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_aggregate_type_aggregate_id_occurred_on",
                schema: "domainEvents",
                table: "DomainEvents",
                columns: new[] { "aggregate_type", "aggregate_id", "occurred_on" });

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_correlation_id",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "correlation_id");

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_tenant_id",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_user_id",
                schema: "domainEvents",
                table: "DomainEvents",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_inbox_messages_processed_at",
                schema: "inbox_messages",
                table: "inbox_messages",
                column: "processed_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_inbox_messages_user_id",
                schema: "inbox_messages",
                table: "inbox_messages",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_otp_challenges_subject_purpose",
                schema: "application",
                table: "OtpChallenges",
                columns: new[] { "subject", "purpose" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_at",
                schema: "outbox_messages",
                table: "outbox_messages",
                column: "processed_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_user_id",
                schema: "outbox_messages",
                table: "outbox_messages",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvents",
                schema: "domainEvents");

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
