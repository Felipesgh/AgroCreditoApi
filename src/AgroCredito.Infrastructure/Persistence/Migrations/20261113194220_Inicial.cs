using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AgroCredito.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Oportunidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    Instituicao = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    FonteOficial = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Ativa = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TaxaJurosAnual = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: true),
                    ValorMaximo = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    PrazoMaximoMeses = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Estado = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true),
                    TipoProducao = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    AreaMinimaHectares = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    AreaMaximaHectares = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oportunidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    SenhaHash = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequisitosDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    OportunidadeId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitosDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitosDocumento_Oportunidades_OportunidadeId",
                        column: x => x.OportunidadeId,
                        principalTable: "Oportunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Propriedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    Municipio = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    AreaHectares = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    TipoProducao = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    Cultura = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ProdutorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propriedades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propriedades_Produtores_ProdutorId",
                        column: x => x.ProdutorId,
                        principalTable: "Produtores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Simulacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PropriedadeId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    OportunidadeId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ValorSolicitado = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxaJurosAnual = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    PrazoMeses = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ValorParcela = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPago = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulacoes_Oportunidades_OportunidadeId",
                        column: x => x.OportunidadeId,
                        principalTable: "Oportunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Simulacoes_Propriedades_PropriedadeId",
                        column: x => x.PropriedadeId,
                        principalTable: "Propriedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Oportunidades",
                columns: new[] { "Id", "AreaMaximaHectares", "AreaMinimaHectares", "Ativa", "Descricao", "Estado", "FonteOficial", "Instituicao", "Nome", "PrazoMaximoMeses", "TaxaJurosAnual", "Tipo", "TipoProducao", "ValorMaximo" },
                values: new object[,]
                {
                    { 1, 50m, null, 1, "Crédito para custear a safra ou a atividade (insumos, sementes, mão de obra) de agricultores familiares. Dados ilustrativos: confirme as condições no edital oficial.", null, "https://www.gov.br/mda", "Bancos autorizados (Pronaf)", "Pronaf Custeio", 24, 3.00m, "CreditoRural", null, 250000m },
                    { 2, 50m, null, 1, "Crédito para investir na propriedade: máquinas, equipamentos e melhorias para agricultores familiares. Dados ilustrativos: confirme as condições no edital oficial.", null, "https://www.gov.br/mda", "Bancos autorizados (Pronaf)", "Pronaf Investimento", 120, 4.00m, "CreditoRural", null, 200000m },
                    { 3, null, 50m, 1, "Crédito para médios produtores rurais, para custeio e investimento. Dados ilustrativos: confirme as condições no edital oficial.", null, "https://www.gov.br/agricultura", "Bancos autorizados (Pronamp)", "Pronamp", 96, 8.00m, "CreditoRural", null, 1500000m },
                    { 4, null, null, 1, "Seguro rural com subvenção federal ao prêmio, para proteger a produção contra perdas climáticas. Dados ilustrativos: confirme as condições no edital oficial.", null, "https://www.gov.br/agricultura", "Seguradoras habilitadas (PSR)", "Seguro Rural (PSR)", null, null, "Seguro", null, null },
                    { 5, 50m, null, 1, "Compra de alimentos produzidos pela agricultura familiar, garantindo mercado e renda ao produtor. Dados ilustrativos: confirme as condições no edital oficial.", null, "https://www.gov.br/mda", "Governo federal (PAA)", "Programa de Aquisição de Alimentos (PAA)", null, null, "ProgramaGoverno", null, null }
                });

            migrationBuilder.InsertData(
                table: "RequisitosDocumento",
                columns: new[] { "Id", "Descricao", "Nome", "OportunidadeId" },
                values: new object[,]
                {
                    { 1, "Cadastro Nacional da Agricultura Familiar.", "CAF", 1 },
                    { 2, "Documentos pessoais do produtor.", "Documento de identificação e CPF", 1 },
                    { 3, "Matrícula, contrato de arrendamento ou documento equivalente.", "Comprovante de posse ou propriedade da terra", 1 },
                    { 4, "Descrição de como o recurso será usado na safra.", "Plano de utilização do crédito", 1 },
                    { 5, "Cadastro Nacional da Agricultura Familiar.", "CAF", 2 },
                    { 6, "Documentos pessoais do produtor.", "Documento de identificação e CPF", 2 },
                    { 7, "Descrição e orçamento do que será comprado ou construído.", "Projeto de investimento", 2 },
                    { 8, "Documentos pessoais do produtor.", "Documento de identificação e CPF", 3 },
                    { 9, "Matrícula, contrato de arrendamento ou documento equivalente.", "Comprovante de posse ou propriedade da terra", 3 },
                    { 10, "Plano de uso do crédito e orçamento.", "Projeto ou orçamento", 3 },
                    { 11, "Proposta feita junto a uma seguradora habilitada.", "Proposta de seguro", 4 },
                    { 12, "Documentos que mostram a atividade e a área segurada.", "Comprovante de enquadramento do produtor", 4 },
                    { 13, "Cadastro Nacional da Agricultura Familiar.", "CAF", 5 },
                    { 14, "Documentos pessoais do produtor.", "Documento de identificação e CPF", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtores_Email",
                table: "Produtores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Propriedades_ProdutorId",
                table: "Propriedades",
                column: "ProdutorId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitosDocumento_OportunidadeId",
                table: "RequisitosDocumento",
                column: "OportunidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_OportunidadeId",
                table: "Simulacoes",
                column: "OportunidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_PropriedadeId",
                table: "Simulacoes",
                column: "PropriedadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequisitosDocumento");

            migrationBuilder.DropTable(
                name: "Simulacoes");

            migrationBuilder.DropTable(
                name: "Oportunidades");

            migrationBuilder.DropTable(
                name: "Propriedades");

            migrationBuilder.DropTable(
                name: "Produtores");
        }
    }
}
