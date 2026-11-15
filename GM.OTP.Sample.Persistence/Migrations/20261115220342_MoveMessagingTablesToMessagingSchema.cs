using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.OTP.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveMessagingTablesToMessagingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.RenameTable(
                name: "outbox_messages",
                schema: "outbox_messages",
                newName: "outbox_messages",
                newSchema: "messaging");

            migrationBuilder.RenameTable(
                name: "inbox_messages",
                schema: "inbox_messages",
                newName: "inbox_messages",
                newSchema: "messaging");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inbox_messages");

            migrationBuilder.EnsureSchema(
                name: "outbox_messages");

            migrationBuilder.RenameTable(
                name: "outbox_messages",
                schema: "messaging",
                newName: "outbox_messages",
                newSchema: "outbox_messages");

            migrationBuilder.RenameTable(
                name: "inbox_messages",
                schema: "messaging",
                newName: "inbox_messages",
                newSchema: "inbox_messages");
        }
    }
}
