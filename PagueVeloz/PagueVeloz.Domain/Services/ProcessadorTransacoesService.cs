using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;
using PagueVeloz.Domain.ValueObjects;
using PagueVeloz.Domain.Events;

namespace PagueVeloz.Domain.Services;

public class ProcessadorTransacoesService
{
    private readonly ValidadorSaldoService _validadorSaldo;

    public ProcessadorTransacoesService(ValidadorSaldoService validadorSaldo)
    {
        _validadorSaldo = validadorSaldo;
    }

    public void ProcessarCredito(Conta conta, Transacao transacao)
    {
        if (transacao.Tipo != TipoTransacao.Credito)
            throw new InvalidOperationException("Tipo de transação inválido para crédito.");

        var novoSaldo = conta.SaldoDisponivel.Adicionar(transacao.Valor);
        conta.AtualizarSaldoDisponivel(novoSaldo);
        transacao.ProcessarComSucesso();

        conta.AdicionarEvento(new SaldoAtualizadoEvent(
            conta.Id,
            conta.SaldoDisponivel.Valor,
            conta.SaldoReservado.Valor));

        conta.AdicionarEvento(new TransacaoProcessadaEvent(
            transacao.Id,
            conta.Id,
            TipoTransacao.Credito.ToString(),
            transacao.Valor));
    }

    public void ProcessarDebito(Conta conta, Transacao transacao)
    {
        if (transacao.Tipo != TipoTransacao.Debito)
            throw new InvalidOperationException("Tipo de transação inválido para débito.");

        if (!_validadorSaldo.ValidarSaldoParaDebito(conta, transacao.Valor))
            throw new InvalidOperationException("Saldo insuficiente para débito.");

        var valorDebito = transacao.Valor;
        var saldoDisponivel = conta.SaldoDisponivel.Valor;

        if (saldoDisponivel >= valorDebito)
        {
            var novoSaldo = conta.SaldoDisponivel.Subtrair(valorDebito);
            conta.AtualizarSaldoDisponivel(novoSaldo);
        }
        else
        {
            var diferenca = valorDebito - saldoDisponivel;
            conta.AtualizarSaldoDisponivel(new Saldo(0));
            
            if (diferenca > conta.LimiteCredito.Valor)
                throw new InvalidOperationException("Saldo e limite de crédito insuficientes.");
        }

        transacao.ProcessarComSucesso();

        conta.AdicionarEvento(new SaldoAtualizadoEvent(
            conta.Id,
            conta.SaldoDisponivel.Valor,
            conta.SaldoReservado.Valor));

        conta.AdicionarEvento(new TransacaoProcessadaEvent(
            transacao.Id,
            conta.Id,
            TipoTransacao.Debito.ToString(),
            transacao.Valor));
    }

    public void ProcessarReserva(Conta conta, Transacao transacao)
    {
        if (transacao.Tipo != TipoTransacao.Reserva)
            throw new InvalidOperationException("Tipo de transação inválido para reserva.");

        if (!_validadorSaldo.ValidarSaldoParaReserva(conta, transacao.Valor))
            throw new InvalidOperationException("Saldo disponível insuficiente para reserva.");

        var novoSaldoDisponivel = conta.SaldoDisponivel.Subtrair(transacao.Valor);
        var novoSaldoReservado = conta.SaldoReservado.Adicionar(transacao.Valor);
        conta.AtualizarSaldos(novoSaldoDisponivel, novoSaldoReservado);
        transacao.ProcessarComSucesso();

        conta.AdicionarEvento(new SaldoAtualizadoEvent(
            conta.Id,
            conta.SaldoDisponivel.Valor,
            conta.SaldoReservado.Valor));

        conta.AdicionarEvento(new TransacaoProcessadaEvent(
            transacao.Id,
            conta.Id,
            TipoTransacao.Reserva.ToString(),
            transacao.Valor));
    }

    public void ProcessarCaptura(Conta conta, Transacao transacao)
    {
        if (transacao.Tipo != TipoTransacao.Captura)
            throw new InvalidOperationException("Tipo de transação inválido para captura.");

        if (!_validadorSaldo.ValidarSaldoParaCaptura(conta, transacao.Valor))
            throw new InvalidOperationException("Saldo reservado insuficiente para captura.");

        var novoSaldoReservado = conta.SaldoReservado.Subtrair(transacao.Valor);
        conta.AtualizarSaldoReservado(novoSaldoReservado);
        transacao.ProcessarComSucesso();

        conta.AdicionarEvento(new SaldoAtualizadoEvent(
            conta.Id,
            conta.SaldoDisponivel.Valor,
            conta.SaldoReservado.Valor));

        conta.AdicionarEvento(new TransacaoProcessadaEvent(
            transacao.Id,
            conta.Id,
            TipoTransacao.Captura.ToString(),
            transacao.Valor));
    }

    public void ProcessarEstorno(Conta conta, Transacao transacaoOriginal, Transacao transacaoEstorno)
    {
        if (transacaoEstorno.Tipo != TipoTransacao.Estorno)
            throw new InvalidOperationException("Tipo de transação inválido para estorno.");

        switch (transacaoOriginal.Tipo)
        {
            case TipoTransacao.Debito:
                var novoSaldoDebito = conta.SaldoDisponivel.Adicionar(transacaoOriginal.Valor);
                conta.AtualizarSaldoDisponivel(novoSaldoDebito);
                break;
            case TipoTransacao.Reserva:
                var novoSaldoReservadoEstorno = conta.SaldoReservado.Subtrair(transacaoOriginal.Valor);
                var novoSaldoDisponivelEstorno = conta.SaldoDisponivel.Adicionar(transacaoOriginal.Valor);
                conta.AtualizarSaldos(novoSaldoDisponivelEstorno, novoSaldoReservadoEstorno);
                break;
            case TipoTransacao.Captura:
                var novoSaldoReservadoCaptura = conta.SaldoReservado.Adicionar(transacaoOriginal.Valor);
                conta.AtualizarSaldoReservado(novoSaldoReservadoCaptura);
                break;
            default:
                throw new InvalidOperationException($"Estorno não permitido para transação do tipo {transacaoOriginal.Tipo}.");
        }

        transacaoEstorno.ProcessarComSucesso();
        transacaoOriginal.Estornar();

        conta.AdicionarEvento(new SaldoAtualizadoEvent(
            conta.Id,
            conta.SaldoDisponivel.Valor,
            conta.SaldoReservado.Valor));

        conta.AdicionarEvento(new TransacaoProcessadaEvent(
            transacaoEstorno.Id,
            conta.Id,
            TipoTransacao.Estorno.ToString(),
            transacaoOriginal.Valor));
    }

    public void ProcessarTransferencia(Conta contaOrigem, Conta contaDestino, Transacao transacao)
    {
        if (transacao.Tipo != TipoTransacao.Transferencia)
            throw new InvalidOperationException("Tipo de transação inválido para transferência.");

        if (!_validadorSaldo.ValidarSaldoParaTransferencia(contaOrigem, contaDestino, transacao.Valor))
            throw new InvalidOperationException("Saldo insuficiente para transferência.");

        // Débito na conta origem
        var valorDebito = transacao.Valor;
        var saldoDisponivel = contaOrigem.SaldoDisponivel.Valor;

        if (saldoDisponivel >= valorDebito)
        {
            var novoSaldoOrigem = contaOrigem.SaldoDisponivel.Subtrair(valorDebito);
            contaOrigem.AtualizarSaldoDisponivel(novoSaldoOrigem);
        }
        else
        {
            var diferenca = valorDebito - saldoDisponivel;
            contaOrigem.AtualizarSaldoDisponivel(new Saldo(0));
        }

        // Crédito na conta destino
        var novoSaldoDestino = contaDestino.SaldoDisponivel.Adicionar(transacao.Valor);
        contaDestino.AtualizarSaldoDisponivel(novoSaldoDestino);

        transacao.ProcessarComSucesso();

        contaOrigem.AdicionarEvento(new SaldoAtualizadoEvent(
            contaOrigem.Id,
            contaOrigem.SaldoDisponivel.Valor,
            contaOrigem.SaldoReservado.Valor));

        contaDestino.AdicionarEvento(new SaldoAtualizadoEvent(
            contaDestino.Id,
            contaDestino.SaldoDisponivel.Valor,
            contaDestino.SaldoReservado.Valor));

        contaOrigem.AdicionarEvento(new TransacaoProcessadaEvent(
            transacao.Id,
            contaOrigem.Id,
            TipoTransacao.Transferencia.ToString(),
            transacao.Valor,
            contaDestino.Id));
    }
}

