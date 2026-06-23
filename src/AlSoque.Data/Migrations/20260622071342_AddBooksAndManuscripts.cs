using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AlSoque.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBooksAndManuscripts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PublicationYear = table.Column<int>(type: "integer", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    ScholarId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Scholars_ScholarId",
                        column: x => x.ScholarId,
                        principalTable: "Scholars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Manuscripts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    EstimatedYear = table.Column<int>(type: "integer", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    ScholarId = table.Column<int>(type: "integer", nullable: true),
                    FamilyId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manuscripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manuscripts_Families_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "Families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Manuscripts_Scholars_ScholarId",
                        column: x => x.ScholarId,
                        principalTable: "Scholars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_ScholarId",
                table: "Books",
                column: "ScholarId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Slug",
                table: "Books",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manuscripts_FamilyId",
                table: "Manuscripts",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscripts_ScholarId",
                table: "Manuscripts",
                column: "ScholarId");

            migrationBuilder.CreateIndex(
                name: "IX_Manuscripts_Slug",
                table: "Manuscripts",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Manuscripts");
        }
    }
}
