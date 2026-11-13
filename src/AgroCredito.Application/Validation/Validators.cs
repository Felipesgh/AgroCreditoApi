using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;

namespace AgroCredito.Application.Validation;

internal sealed class ErrorCollector
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public void Add(string campo, string mensagem)
    {
        if (!_errors.TryGetValue(campo, out var lista))
        {
            lista = new List<string>();
            _errors[campo] = lista;
        }
        lista.Add(mensagem);
    }

    public void ThrowIfAny()
    {
        if (_errors.Count == 0)
            return;

        throw new ValidationException(_errors.ToDictionary(e => e.Key, e => e.Value.ToArray()));
    }
}

internal static class PropriedadeValidator
{
    public static void Validar(PropriedadeRequest r)
    {
        var erros = new ErrorCollector();

        if (string.IsNullOrWhiteSpace(r.Nome))
            erros.Add(nameof(r.Nome), "O nome é obrigatório.");
        else if (r.Nome.Trim().Length > 150)
            erros.Add(nameof(r.Nome), "O nome deve ter no máximo 150 caracteres.");

        var estado = r.Estado?.Trim();
        if (string.IsNullOrEmpty(estado) || estado.Length != 2 || !estado.All(char.IsLetter))
            erros.Add(nameof(r.Estado), "Informe a UF com 2 letras (ex.: SP).");

        if (string.IsNullOrWhiteSpace(r.Municipio))
            erros.Add(nameof(r.Municipio), "O município é obrigatório.");
        else if (r.Municipio.Trim().Length > 100)
            erros.Add(nameof(r.Municipio), "O município deve ter no máximo 100 caracteres.");

        if (r.AreaHectares <= 0)
            erros.Add(nameof(r.AreaHectares), "A área deve ser maior que zero.");
        else if (r.AreaHectares > 100000)
            erros.Add(nameof(r.AreaHectares), "A área informada é inválida (máximo de 100.000 ha).");

        if (!Enum.IsDefined(r.TipoProducao))
            erros.Add(nameof(r.TipoProducao), "Tipo de produção inválido.");

        if (string.IsNullOrWhiteSpace(r.Cultura))
            erros.Add(nameof(r.Cultura), "A cultura é obrigatória.");
        else if (r.Cultura.Trim().Length > 100)
            erros.Add(nameof(r.Cultura), "A cultura deve ter no máximo 100 caracteres.");

        erros.ThrowIfAny();
    }
}

internal static class SimulacaoValidator
{
    public static void Validar(SimulacaoRequest r)
    {
        var erros = new ErrorCollector();

        if (r.OportunidadeId <= 0)
            erros.Add(nameof(r.OportunidadeId), "Informe a oportunidade.");

        if (r.Valor <= 0)
            erros.Add(nameof(r.Valor), "O valor deve ser maior que zero.");

        if (r.PrazoMeses < 1 || r.PrazoMeses > 480)
            erros.Add(nameof(r.PrazoMeses), "O prazo deve ficar entre 1 e 480 meses.");

        erros.ThrowIfAny();
    }
}
