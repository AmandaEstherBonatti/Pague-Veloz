namespace PagueVeloz.Domain.Events;

public class TransacaoProcessadaEvent : IDomainEvent
{
    public Guid Id { get; }
    public Guid TransacaoId { get; }
    public Guid ContaId { get; }
    public Guid? ContaDestinoId { get; }
    public string TipoTransacao { get; }
    public decimal Valor { get; }
    public DateTime OcorreuEm { get; }

    public TransacaoProcessadaEvent(
        Guid transacaoId,
        Guid contaId,
        string tipoTransacao,
        decimal valor,
        Guid? contaDestinoId = null)
    {
        Id = Guid.NewGuid();
        TransacaoId = transacaoId;
        ContaId = contaId;
        ContaDestinoId = contaDestinoId;
        TipoTransacao = tipoTransacao;
        Valor = valor;
        OcorreuEm = DateTime.UtcNow;
    }
}

