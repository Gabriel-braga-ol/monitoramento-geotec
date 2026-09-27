using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Geotrend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barragem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Localizacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NivelAlertaAtual = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barragem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instrumentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    LimiteAtencao = table.Column<double>(type: "double precision", nullable: false),
                    LimiteAlerta = table.Column<double>(type: "double precision", nullable: false),
                    LimiteEmergencia = table.Column<double>(type: "double precision", nullable: false),
                    BarragemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instrumentos_Barragem_BarragemId",
                        column: x => x.BarragemId,
                        principalTable: "Barragem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leituras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstrumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Valor = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leituras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leituras_Instrumentos_InstrumentoId",
                        column: x => x.InstrumentoId,
                        principalTable: "Instrumentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instrumentos_BarragemId",
                table: "Instrumentos",
                column: "BarragemId");

            migrationBuilder.CreateIndex(
                name: "IX_Leituras_InstrumentoId",
                table: "Leituras",
                column: "InstrumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leituras");

            migrationBuilder.DropTable(
                name: "Instrumentos");

            migrationBuilder.DropTable(
                name: "Barragem");
        }
    }
}
