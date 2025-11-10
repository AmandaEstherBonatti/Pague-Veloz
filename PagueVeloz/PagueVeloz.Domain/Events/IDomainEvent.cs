namespace PagueVeloz.Domain.Events;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OcorreuEm { get; }
}

