namespace PagueVeloz.Domain.ValueObjects;

public class Saldo
{
    public decimal Valor { get; private set; }

    public Saldo(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("O saldo não pode ser negativo.", nameof(valor));

        Valor = valor;
    }

    public Saldo Adicionar(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("O valor a adicionar não pode ser negativo.", nameof(valor));

        return new Saldo(Valor + valor);
    }

    public Saldo Subtrair(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("O valor a subtrair não pode ser negativo.", nameof(valor));

        var novoValor = Valor - valor;
        if (novoValor < 0)
            throw new InvalidOperationException("Saldo insuficiente.");

        return new Saldo(novoValor);
    }

    public bool PodeSubtrair(decimal valor)
    {
        return Valor >= valor;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Saldo outro)
            return false;

        return Valor == outro.Valor;
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public static implicit operator decimal(Saldo saldo) => saldo.Valor;
}

