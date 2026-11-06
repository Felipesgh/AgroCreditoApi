using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Enums;
using AgroCredito.Domain.Services;
using Xunit;

namespace AgroCredito.Domain.Tests;

public class ElegibilidadeDomainServiceTests
{
    private readonly ElegibilidadeDomainService _service = new();

    private static Propriedade NovaPropriedade() => new()
    {
        Nome = "SÃ­tio Boa Vista",
        Estado = "SP",
        Municipio = "IbiÃºna",
        AreaHectares = 15,
        TipoProducao = TipoProducao.Horticultura,
        Cultura = "Alface"
    };

    [Fact]
    public void Oportunidade_sem_restricoes_e_elegivel()
    {
        var oportunidade = new Oportunidade { Nome = "Linha aberta" };

        Assert.True(_service.EhElegivel(NovaPropriedade(), oportunidade));
    }

    [Fact]
    public void Oportunidade_inativa_nao_e_elegivel()
    {
        var oportunidade = new Oportunidade { Nome = "Encerrada", Ativa = false };

        Assert.False(_service.EhElegivel(NovaPropriedade(), oportunidade));
    }

    [Fact]
    public void Estado_diferente_nao_e_elegivel()
    {
        var oportunidade = new Oportunidade { Nome = "Programa MG", Estado = "MG" };

        Assert.False(_service.EhElegivel(NovaPropriedade(), oportunidade));
    }

    [Fact]
    public void Estado_igual_ignora_maiusculas()
    {
        var oportunidade = new Oportunidade { Nome = "Programa SP", Estado = "sp" };

        Assert.True(_service.EhElegivel(NovaPropriedade(), oportunidade));
    }

    [Fact]
    public void Tipo_de_producao_diferente_nao_e_elegivel()
    {
        var oportunidade = new Oportunidade { Nome = "PecuÃ¡ria", TipoProducao = TipoProducao.Pecuaria };

        Assert.False(_service.EhElegivel(NovaPropriedade(), oportunidade));
    }

    [Theory]
    [InlineData(20, null, false)]   // Ã¡rea mÃ­nima 20 ha, propriedade tem 15
    [InlineData(10, null, true)]
    [InlineData(null, 10, false)]   // Ã¡rea mÃ¡xima 10 ha, propriedade tem 15
    [InlineData(null, 15, true)]    // limite exato Ã© permitido
    public void Respeita_limites_de_area(int? minima, int? maxima, bool esperado)
    {
        var oportunidade = new Oportunidade
        {
            Nome = "Por Ã¡rea",
            AreaMinimaHectares = (decimal?)minima,
            AreaMaximaHectares = (decimal?)maxima
        };

        Assert.Equal(esperado, _service.EhElegivel(NovaPropriedade(), oportunidade));
    }

    [Fact]
    public void FiltrarElegiveis_retorna_somente_as_compativeis()
    {
        var compativel = new Oportunidade { Nome = "SP", Estado = "SP" };
        var incompativel = new Oportunidade { Nome = "MG", Estado = "MG" };

        var resultado = _service.FiltrarElegiveis(NovaPropriedade(), new[] { compativel, incompativel }).ToList();

        Assert.Single(resultado);
        Assert.Equal("SP", resultado[0].Nome);
    }
}
