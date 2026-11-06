using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Enums;

namespace AgroCredito.Infrastructure.Persistence;

/// <summary>
/// Dados ILUSTRATIVOS para demonstração. Taxas, prazos e limites devem ser
/// conferidos nos editais oficiais (Plano Safra vigente) antes de qualquer uso real.
/// </summary>
public static class SeedData
{
    private const string Aviso = " Dados ilustrativos: confirme as condições no edital oficial.";

    public static Oportunidade[] Oportunidades =>
    [
        new()
        {
            Id = 1, Nome = "Pronaf Custeio", Tipo = TipoOportunidade.CreditoRural,
            Instituicao = "Bancos autorizados (Pronaf)",
            Descricao = "Crédito para custear a safra ou a atividade (insumos, sementes, mão de obra) de agricultores familiares." + Aviso,
            FonteOficial = "https://www.gov.br/mda",
            TaxaJurosAnual = 3.00m, ValorMaximo = 250000m, PrazoMaximoMeses = 24, AreaMaximaHectares = 50m
        },
        new()
        {
            Id = 2, Nome = "Pronaf Investimento", Tipo = TipoOportunidade.CreditoRural,
            Instituicao = "Bancos autorizados (Pronaf)",
            Descricao = "Crédito para investir na propriedade: máquinas, equipamentos e melhorias para agricultores familiares." + Aviso,
            FonteOficial = "https://www.gov.br/mda",
            TaxaJurosAnual = 4.00m, ValorMaximo = 200000m, PrazoMaximoMeses = 120, AreaMaximaHectares = 50m
        },
        new()
        {
            Id = 3, Nome = "Pronamp", Tipo = TipoOportunidade.CreditoRural,
            Instituicao = "Bancos autorizados (Pronamp)",
            Descricao = "Crédito para médios produtores rurais, para custeio e investimento." + Aviso,
            FonteOficial = "https://www.gov.br/agricultura",
            TaxaJurosAnual = 8.00m, ValorMaximo = 1500000m, PrazoMaximoMeses = 96, AreaMinimaHectares = 50m
        },
        new()
        {
            Id = 4, Nome = "Seguro Rural (PSR)", Tipo = TipoOportunidade.Seguro,
            Instituicao = "Seguradoras habilitadas (PSR)",
            Descricao = "Seguro rural com subvenção federal ao prêmio, para proteger a produção contra perdas climáticas." + Aviso,
            FonteOficial = "https://www.gov.br/agricultura"
        },
        new()
        {
            Id = 5, Nome = "Programa de Aquisição de Alimentos (PAA)", Tipo = TipoOportunidade.ProgramaGoverno,
            Instituicao = "Governo federal (PAA)",
            Descricao = "Compra de alimentos produzidos pela agricultura familiar, garantindo mercado e renda ao produtor." + Aviso,
            FonteOficial = "https://www.gov.br/mda",
            AreaMaximaHectares = 50m
        }
    ];

    public static RequisitoDocumento[] Requisitos =>
    [
        R(1, 1, "CAF", "Cadastro Nacional da Agricultura Familiar."),
        R(2, 1, "Documento de identificação e CPF", "Documentos pessoais do produtor."),
        R(3, 1, "Comprovante de posse ou propriedade da terra", "Matrícula, contrato de arrendamento ou documento equivalente."),
        R(4, 1, "Plano de utilização do crédito", "Descrição de como o recurso será usado na safra."),

        R(5, 2, "CAF", "Cadastro Nacional da Agricultura Familiar."),
        R(6, 2, "Documento de identificação e CPF", "Documentos pessoais do produtor."),
        R(7, 2, "Projeto de investimento", "Descrição e orçamento do que será comprado ou construído."),

        R(8, 3, "Documento de identificação e CPF", "Documentos pessoais do produtor."),
        R(9, 3, "Comprovante de posse ou propriedade da terra", "Matrícula, contrato de arrendamento ou documento equivalente."),
        R(10, 3, "Projeto ou orçamento", "Plano de uso do crédito e orçamento."),

        R(11, 4, "Proposta de seguro", "Proposta feita junto a uma seguradora habilitada."),
        R(12, 4, "Comprovante de enquadramento do produtor", "Documentos que mostram a atividade e a área segurada."),

        R(13, 5, "CAF", "Cadastro Nacional da Agricultura Familiar."),
        R(14, 5, "Documento de identificação e CPF", "Documentos pessoais do produtor.")
    ];

    private static RequisitoDocumento R(int id, int oportunidadeId, string nome, string descricao) => new()
    {
        Id = id,
        OportunidadeId = oportunidadeId,
        Nome = nome,
        Descricao = descricao
    };
}
