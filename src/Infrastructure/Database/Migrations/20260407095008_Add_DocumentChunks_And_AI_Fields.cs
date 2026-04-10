using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class Add_DocumentChunks_And_AI_Fields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:PostgresExtension:vector", ",,");

        migrationBuilder.AddColumn<string>(
            name: "classification",
            schema: "public",
            table: "documents",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "summary",
            schema: "public",
            table: "documents",
            type: "character varying(4000)",
            maxLength: 4000,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "document_chunks",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                document_id = table.Column<Guid>(type: "uuid", nullable: false),
                chunk_index = table.Column<int>(type: "integer", nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                token_count = table.Column<int>(type: "integer", nullable: false),
                embedding_vector = table.Column<Vector>(type: "vector(1536)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_document_chunks", x => x.id);
                table.ForeignKey(
                    name: "fk_document_chunks_documents_document_id",
                    column: x => x.document_id,
                    principalSchema: "public",
                    principalTable: "documents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_document_chunks_document_id",
            schema: "public",
            table: "document_chunks",
            column: "document_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "document_chunks",
            schema: "public");

        migrationBuilder.DropColumn(
            name: "classification",
            schema: "public",
            table: "documents");

        migrationBuilder.DropColumn(
            name: "summary",
            schema: "public",
            table: "documents");

        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
    }
}
