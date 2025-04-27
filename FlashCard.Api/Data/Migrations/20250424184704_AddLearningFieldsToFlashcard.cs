using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashCard.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningFieldsToFlashcard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_Generations_GenerationId",
                table: "Flashcards");

            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswersInRow",
                table: "Flashcards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLearned",
                table: "Flashcards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReviewedAt",
                table: "Flashcards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextReviewAt",
                table: "Flashcards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_Generations_GenerationId",
                table: "Flashcards",
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

            migrationBuilder.DropColumn(
                name: "CorrectAnswersInRow",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "IsLearned",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "LastReviewedAt",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "NextReviewAt",
                table: "Flashcards");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_Generations_GenerationId",
                table: "Flashcards",
                column: "GenerationId",
                principalTable: "Generations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
