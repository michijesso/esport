using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Esport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "markets",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_markets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    current_score = table.Column<string>(type: "text", nullable: false),
                    MarketId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_events_markets_MarketId",
                        column: x => x.MarketId,
                        principalSchema: "public",
                        principalTable: "markets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "selections",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    odds = table.Column<double>(type: "double precision", nullable: false),
                    MarketId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selections", x => x.id);
                    table.ForeignKey(
                        name: "FK_selections_markets_MarketId",
                        column: x => x.MarketId,
                        principalSchema: "public",
                        principalTable: "markets",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "esport_events",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    esport = table.Column<string>(type: "text", nullable: false),
                    league = table.Column<string>(type: "text", nullable: false),
                    championship = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_esport_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_esport_events_events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "participants",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participants", x => x.id);
                    table.ForeignKey(
                        name: "FK_participants_events_EventId",
                        column: x => x.EventId,
                        principalSchema: "public",
                        principalTable: "events",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_esport_events_EventId",
                schema: "public",
                table: "esport_events",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_events_MarketId",
                schema: "public",
                table: "events",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_participants_EventId",
                schema: "public",
                table: "participants",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_selections_MarketId",
                schema: "public",
                table: "selections",
                column: "MarketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "esport_events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "participants",
                schema: "public");

            migrationBuilder.DropTable(
                name: "selections",
                schema: "public");

            migrationBuilder.DropTable(
                name: "events",
                schema: "public");

            migrationBuilder.DropTable(
                name: "markets",
                schema: "public");
        }
    }
}
