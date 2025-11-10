using System.Security.Claims;

namespace PagueVeloz.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(Guid clienteId, string usuario, string email);
    ClaimsPrincipal? ValidateToken(string token);
}

