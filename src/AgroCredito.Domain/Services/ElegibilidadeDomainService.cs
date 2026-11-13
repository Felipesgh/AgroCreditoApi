using AgroCredito.Domain.Entities;

namespace AgroCredito.Domain.Services;

public class ElegibilidadeDomainService : IElegibilidadeDomainService
{
    public bool EhElegivel(Propriedade propriedade, Oportunidade oportunidade)
    {
        if (!oportunidade.Ativa)
            return false;

        if (!string.IsNullOrWhiteSpace(oportunidade.Estado) &&
            !string.Equals(oportunidade.Estado, propriedade.Estado, StringComparison.OrdinalIgnoreCase))
            return false;

        if (oportunidade.TipoProducao.HasValue && oportunidade.TipoProducao.Value != propriedade.TipoProducao)
            return false;

        if (oportunidade.AreaMinimaHectares.HasValue && propriedade.AreaHectares < oportunidade.AreaMinimaHectares.Value)
            return false;

        if (oportunidade.AreaMaximaHectares.HasValue && propriedade.AreaHectares > oportunidade.AreaMaximaHectares.Value)
            return false;

        return true;
    }

    public IEnumerable<Oportunidade> FiltrarElegiveis(Propriedade propriedade, IEnumerable<Oportunidade> oportunidades)
        => oportunidades.Where(oportunidade => EhElegivel(propriedade, oportunidade));
}
