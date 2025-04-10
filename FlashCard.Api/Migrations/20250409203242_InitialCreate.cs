using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCard.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenerationErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceTextHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceTextLength = table.Column<int>(type: "int", nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationErrorLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerationErrorLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Generations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GeneratedCount = table.Column<int>(type: "int", nullable: false),
                    AcceptedUneditedCount = table.Column<int>(type: "int", nullable: true),
                    AcceptedEditedCount = table.Column<int>(type: "int", nullable: true),
                    SourceTextHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SourceTextLength = table.Column<int>(type: "int", nullable: false),
                    GenerationDuration = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Generations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flashcards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GenerationId = table.Column<int>(type: "int", nullable: true),
                    Front = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Back = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flashcards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flashcards_Generations_GenerationId",
                        column: x => x.GenerationId,
                        principalTable: "Generations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flashcards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_GenerationId",
                table: "Flashcards",
                column: "GenerationId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_Source",
                table: "Flashcards",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_UserId",
                table: "Flashcards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerationErrorLogs_UserId",
                table: "GenerationErrorLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Generations_UserId",
                table: "Generations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flashcards");

            migrationBuilder.DropTable(
                name: "GenerationErrorLogs");

            migrationBuilder.DropTable(
                name: "Generations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
