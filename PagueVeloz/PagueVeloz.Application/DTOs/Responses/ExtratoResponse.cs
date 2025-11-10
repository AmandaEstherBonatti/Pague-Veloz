using PagueVeloz.Application.DTOs;

namespace PagueVeloz.Application.DTOs.Responses;

public class ExtratoResponse
{
    public Guid ContaId { get; set; }
    public string NumeroConta { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public List<TransacaoDto> Transacoes { get; set; } = new();
    public int TotalTransacoes { get; set; }
}

