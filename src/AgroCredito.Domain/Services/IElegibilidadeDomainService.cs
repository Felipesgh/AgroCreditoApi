using AgroCredito.Domain.Entities;

namespace AgroCredito.Domain.Services;

public interface IElegibilidadeDomainService
{
    bool EhElegivel(Propriedade propriedade, Oportunidade oportunidade);
    IEnumerable<Oportunidade> FiltrarElegiveis(Propriedade propriedade, IEnumerable<Oportunidade> oportunidades);
}