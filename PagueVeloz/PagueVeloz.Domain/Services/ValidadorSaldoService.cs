using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Domain.Services;

public class ValidadorSaldoService
{
    public bool ValidarSaldoParaDebito(Conta conta, decimal valor)
    {
        if (conta.Status != StatusConta.Ativa)
            return false;

        var saldoTotal = conta.CalcularSaldoTotal();
        var limiteDisponivel = conta.CalcularLimiteDisponivel();

        return saldoTotal + limiteDisponivel >= valor;
    }

    public bool ValidarSaldoParaReserva(Conta conta, decimal valor)
    {
        if (conta.Status != StatusConta.Ativa)
            return false;

        return conta.SaldoDisponivel.PodeSubtrair(valor);
    }

    public bool ValidarSaldoParaCaptura(Conta conta, decimal valor)
    {
        if (conta.Status != StatusConta.Ativa)
            return false;

        return conta.SaldoReservado.PodeSubtrair(valor);
    }

    public bool ValidarSaldoParaTransferencia(Conta contaOrigem, Conta contaDestino, decimal valor)
    {
        if (contaOrigem.Status != StatusConta.Ativa || contaDestino.Status != StatusConta.Ativa)
            return false;

        return ValidarSaldoParaDebito(contaOrigem, valor);
    }
}

