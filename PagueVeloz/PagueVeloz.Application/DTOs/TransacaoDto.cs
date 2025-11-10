namespace PagueVeloz.Application.DTOs;

public class TransacaoDto
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid? ContaDestinoId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataProcessamento { get; set; }
    public string? Erro { get; set; }
}
