using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCard.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_Generations_GenerationId",
                table: "Flashcards");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_Source",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "AcceptedEditedCount",
                table: "Generations");

            migrationBuilder.DropColumn(
                name: "AcceptedUneditedCount",
                table: "Generations");

            migrationBuilder.DropColumn(
                name: "GenerationDuration",
                table: "Generations");

            migrationBuilder.DropColumn(
                name: "SourceTextLength",
                table: "Generations");

            migrationBuilder.DropColumn(
                name: "SourceTextLength",
                table: "GenerationErrorLogs");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Generations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ErrorDetails",
                table: "GenerationErrorLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GenerationId",
                table: "GenerationErrorLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenerationErrorLogs_GenerationId",
                table: "GenerationErrorLogs",
                column: "GenerationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_Generations_GenerationId",
                table: "Flashcards",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_GenerationErrorLogs_Generations_GenerationId",
                table: "GenerationErrorLogs",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_Generations_GenerationId",
                table: "Flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_GenerationErrorLogs_Generations_GenerationId",
                table: "GenerationErrorLogs");

            migrationBuilder.DropIndex(
                name: "IX_GenerationErrorLogs_GenerationId",
                table: "GenerationErrorLogs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Generations");

            migrationBuilder.DropColumn(
                name: "ErrorDetails",
                table: "GenerationErrorLogs");

            migrationBuilder.DropColumn(
                name: "GenerationId",
                table: "GenerationErrorLogs");

            migrationBuilder.AddColumn<int>(
                name: "AcceptedEditedCount",
                table: "Generations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcceptedUneditedCount",
                table: "Generations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenerationDuration",
                table: "Generations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceTextLength",
                table: "Generations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceTextLength",
                table: "GenerationErrorLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_Source",
                table: "Flashcards",
                column: "Source");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_Generations_GenerationId",
                table: "Flashcards",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id");
        }
    }
}
