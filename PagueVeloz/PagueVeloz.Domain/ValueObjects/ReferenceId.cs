namespace PagueVeloz.Domain.ValueObjects;

public class ReferenceId
{
    public string Valor { get; private set; }

    public ReferenceId(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O ReferenceId não pode ser vazio.", nameof(valor));

        Valor = valor;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ReferenceId outro)
            return false;

        return Valor == outro.Valor;
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public override string ToString() => Valor;

    public static implicit operator string(ReferenceId referenceId) => referenceId.Valor;
}

