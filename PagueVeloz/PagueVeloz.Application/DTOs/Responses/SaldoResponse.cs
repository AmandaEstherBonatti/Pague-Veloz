namespace PagueVeloz.Application.DTOs.Responses;

public class SaldoResponse
{
    public Guid ContaId { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public decimal SaldoDisponivel { get; set; }
    public decimal SaldoReservado { get; set; }
    public decimal SaldoTotal { get; set; }
    public decimal LimiteCredito { get; set; }
    public decimal LimiteDisponivel { get; set; }
}

