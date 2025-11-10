namespace PagueVeloz.Application.DTOs.Responses;

public class LoginResponse
{
    public Guid ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public bool RequerAutenticacaoMultiFator { get; set; }
}

