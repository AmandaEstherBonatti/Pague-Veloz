namespace PagueVeloz.Domain.Events;

public class SaldoAtualizadoEvent : IDomainEvent
{
    public Guid Id { get; }
    public Guid ContaId { get; }
    public decimal SaldoDisponivel { get; }
    public decimal SaldoReservado { get; }
    public decimal SaldoTotal { get; }
    public DateTime OcorreuEm { get; }

    public SaldoAtualizadoEvent(
        Guid contaId,
        decimal saldoDisponivel,
        decimal saldoReservado)
    {
        Id = Guid.NewGuid();
        ContaId = contaId;
        SaldoDisponivel = saldoDisponivel;
        SaldoReservado = saldoReservado;
        SaldoTotal = saldoDisponivel + saldoReservado;
        OcorreuEm = DateTime.UtcNow;
    }
}

