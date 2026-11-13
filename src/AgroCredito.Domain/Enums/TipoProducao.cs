using System.Text.Json.Serialization;

namespace AgroCredito.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TipoProducao
{
    Agricultura = 1,
    Horticultura = 2,
    Fruticultura = 3,
    Pecuaria = 4,
    Misto = 5
}
