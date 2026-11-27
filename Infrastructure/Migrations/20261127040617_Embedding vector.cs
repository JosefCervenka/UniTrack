using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Embeddingvector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "LostLocation",
                table: "LostItem",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Vector>(
                name: "Embedding",
                table: "LostItem",
                type: "vector(384)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LostItem_Embedding",
                table: "LostItem",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LostItem_Embedding",
                table: "LostItem");

            migrationBuilder.DropColumn(
                name: "Embedding",
                table: "LostItem");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.AlterColumn<string>(
                name: "LostLocation",
                table: "LostItem",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
