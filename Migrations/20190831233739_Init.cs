using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BotWithAPI.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LastDdCheck = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StickerFloodControl = table.Column<bool>(nullable: false),
                    StickerFloodIgnoreAdmins = table.Column<bool>(nullable: false),
                    StickerFloodDelay = table.Column<int>(nullable: false),
                    ReputationEnabled = table.Column<bool>(nullable: false),
                    BayanAnimation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatParams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Rules = table.Column<string>(nullable: true),
                    Chat = table.Column<long>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LastUpDateTime = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    SettingsId = table.Column<int>(nullable: true),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatParams_ChatSettings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "ChatSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserParams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Warns = table.Column<short>(nullable: false),
                    Reputation = table.Column<int>(nullable: false),
                    Chat = table.Column<long>(nullable: false),
                    User = table.Column<long>(nullable: false),
                    ChatParamsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserParams_ChatParams_ChatParamsId",
                        column: x => x.ChatParamsId,
                        principalTable: "ChatParams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatParams_SettingsId",
                table: "ChatParams",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParams_ChatParamsId",
                table: "UserParams",
                column: "ChatParamsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserParams_User_Chat",
                table: "UserParams",
                columns: new[] { "User", "Chat" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotSettings");

            migrationBuilder.DropTable(
                name: "UserParams");

            migrationBuilder.DropTable(
                name: "ChatParams");

            migrationBuilder.DropTable(
                name: "ChatSettings");
        }
    }
}
