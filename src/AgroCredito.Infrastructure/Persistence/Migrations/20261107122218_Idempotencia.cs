using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroCredito.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Idempotencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdempotencyRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ProdutorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    RequestHash = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false),
                    StatusCode = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ResponseBody = table.Column<string>(type: "NCLOB", nullable: true),
                    Location = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    CriadaEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotencyRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_ProdutorId_IdempotencyKey",
                table: "IdempotencyRecords",
                columns: new[] { "ProdutorId", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdempotencyRecords");
        }
    }
}
