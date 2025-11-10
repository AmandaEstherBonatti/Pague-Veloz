namespace PagueVeloz.Domain.ValueObjects;

public class LimiteCredito
{
    public decimal Valor { get; private set; }

    public LimiteCredito(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("O limite de crédito não pode ser negativo.", nameof(valor));

        Valor = valor;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not LimiteCredito outro)
            return false;

        return Valor == outro.Valor;
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public static implicit operator decimal(LimiteCredito limite) => limite.Valor;
}

