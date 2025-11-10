namespace PagueVeloz.Domain.Events;

public class ContaDesbloqueadaEvent : IDomainEvent
{
    public Guid Id { get; }
    public Guid ContaId { get; }
    public Guid ClienteId { get; }
    public DateTime OcorreuEm { get; }

    public ContaDesbloqueadaEvent(Guid contaId, Guid clienteId, DateTime ocorreuEm)
    {
        Id = Guid.NewGuid();
        ContaId = contaId;
        ClienteId = clienteId;
        OcorreuEm = ocorreuEm;
    }
}

