using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.OTP.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpChallengeUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "application",
                table: "OtpChallenges",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "application",
                table: "OtpChallenges");
        }
    }
}
