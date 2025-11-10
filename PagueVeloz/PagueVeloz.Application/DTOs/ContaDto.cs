namespace PagueVeloz.Application.DTOs;

public class ContaDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public decimal SaldoDisponivel { get; set; }
    public decimal SaldoReservado { get; set; }
    public decimal SaldoTotal { get; set; }
    public decimal LimiteCredito { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
}

