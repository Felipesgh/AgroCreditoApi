using System.Text.Json.Serialization;

namespace AgroCredito.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TipoOportunidade
{
    CreditoRural = 1,
    Seguro = 2,
    ProgramaGoverno = 3,
    Incentivo = 4
}
