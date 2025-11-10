using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Application.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByUsuarioAsync(string usuario, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteUsuarioAsync(string usuario, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
}

