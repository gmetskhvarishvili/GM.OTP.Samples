using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.OTP.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HandOverIngestedInboxRowsToLibraryWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Every inbox row here was ingested by the consumer worker. Rows written before
            // GM.Messaging 2.0 have ingested = false, so mark them for the library inbox worker, which
            // replaces the hand-written InboxProcessorWorker. (Failed rows get retried this way too.)
            migrationBuilder.Sql("UPDATE messaging.inbox_messages SET ingested = TRUE WHERE processed_at_utc IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nothing to undo: the 2.0 migration's Down drops the ingested column.
        }
    }
}
