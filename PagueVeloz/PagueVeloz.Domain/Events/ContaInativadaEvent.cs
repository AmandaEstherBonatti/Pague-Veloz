namespace PagueVeloz.Domain.Events;

public class ContaInativadaEvent : IDomainEvent
{
    public Guid Id { get; }
    public Guid ContaId { get; }
    public Guid ClienteId { get; }
    public DateTime OcorreuEm { get; }

    public ContaInativadaEvent(Guid contaId, Guid clienteId, DateTime ocorreuEm)
    {
        Id = Guid.NewGuid();
        ContaId = contaId;
        ClienteId = clienteId;
        OcorreuEm = ocorreuEm;
    }
}

