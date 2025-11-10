using System.Diagnostics.Metrics;

namespace PagueVeloz.Infrastructure.Metrics;

public class MetricsService
{
    private readonly Meter _meter;
    private readonly Counter<long> _transacoesProcessadas;
    private readonly Counter<long> _transacoesFalhadas;
    private readonly Histogram<double> _tempoProcessamentoTransacao;
    private readonly Counter<long> _clientesCriados;
    private readonly Counter<long> _contasCriadas;

    public MetricsService()
    {
        _meter = new Meter("PagueVeloz", "1.0.0");
        _transacoesProcessadas = _meter.CreateCounter<long>("transacoes_processadas_total", "count", "Total de transações processadas");
        _transacoesFalhadas = _meter.CreateCounter<long>("transacoes_falhadas_total", "count", "Total de transações falhadas");
        _tempoProcessamentoTransacao = _meter.CreateHistogram<double>("tempo_processamento_transacao_seconds", "seconds", "Tempo de processamento de transações");
        _clientesCriados = _meter.CreateCounter<long>("clientes_criados_total", "count", "Total de clientes criados");
        _contasCriadas = _meter.CreateCounter<long>("contas_criadas_total", "count", "Total de contas criadas");
    }

    public void IncrementarTransacaoProcessada(string tipo)
    {
        _transacoesProcessadas.Add(1, new KeyValuePair<string, object?>("tipo", tipo));
    }

    public void IncrementarTransacaoFalhada(string tipo, string erro)
    {
        _transacoesFalhadas.Add(1, 
            new KeyValuePair<string, object?>("tipo", tipo),
            new KeyValuePair<string, object?>("erro", erro));
    }

    public void RegistrarTempoProcessamento(double tempoSegundos, string tipo)
    {
        _tempoProcessamentoTransacao.Record(tempoSegundos, new KeyValuePair<string, object?>("tipo", tipo));
    }

    public void IncrementarClienteCriado()
    {
        _clientesCriados.Add(1);
    }

    public void IncrementarContaCriada()
    {
        _contasCriadas.Add(1);
    }
}

