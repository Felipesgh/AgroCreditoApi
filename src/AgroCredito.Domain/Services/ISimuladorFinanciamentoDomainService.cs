using AgroCredito.Domain.Entities;

namespace AgroCredito.Domain.Services;

public interface ISimuladorFinanciamentoDomainService
{
    ResultadoSimulacao Simular(Oportunidade oportunidade, decimal valor, int prazoMeses);
}
