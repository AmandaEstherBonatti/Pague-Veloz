namespace PagueVeloz.Application.DTOs;

public class ClienteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public bool AutenticacaoMultiFatorAtiva { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativo { get; set; }
}

