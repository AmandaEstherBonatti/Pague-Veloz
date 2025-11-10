using Microsoft.Extensions.Logging;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Domain.Events;

namespace PagueVeloz.Infrastructure.Events;

public class EventBus : IEventBus
{
    private readonly ILogger<EventBus> _logger;

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        _logger.LogInformation(
            "Publicando evento de domínio: {EventType} - {EventId} em {OcorreuEm}",
            typeof(T).Name,
            domainEvent.Id,
            domainEvent.OcorreuEm);

        // TODO: Implementar publicação real (RabbitMQ, Azure Service Bus, etc.)
        // Por enquanto apenas log
        await Task.CompletedTask;
    }

    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await PublishAsync(domainEvent, cancellationToken);
        }
    }
}

