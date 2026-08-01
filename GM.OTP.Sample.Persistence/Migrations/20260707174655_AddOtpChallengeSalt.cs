using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.OTP.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpChallengeSalt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                schema: "application",
                table: "OtpChallenges",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                schema: "application",
                table: "OtpChallenges");
        }
    }
}
