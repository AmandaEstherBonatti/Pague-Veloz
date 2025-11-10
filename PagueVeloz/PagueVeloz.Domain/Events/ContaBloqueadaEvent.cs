namespace PagueVeloz.Domain.Events;

public class ContaBloqueadaEvent : IDomainEvent
{
    public Guid Id { get; }
    public Guid ContaId { get; }
    public Guid ClienteId { get; }
    public DateTime OcorreuEm { get; }

    public ContaBloqueadaEvent(Guid contaId, Guid clienteId, DateTime ocorreuEm)
    {
        Id = Guid.NewGuid();
        ContaId = contaId;
        ClienteId = clienteId;
        OcorreuEm = ocorreuEm;
    }
}

