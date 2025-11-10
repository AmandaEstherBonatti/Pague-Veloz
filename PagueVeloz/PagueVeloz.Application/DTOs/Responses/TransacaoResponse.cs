namespace PagueVeloz.Application.DTOs.Responses;

/// <summary>
/// Response de transação conforme especificação do desafio
/// </summary>
public class TransacaoResponse
{
    /// <summary>
    /// Identificador único da transação processada
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Status da operação: success, failed, pending
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Saldo total da conta após a operação
    /// </summary>
    public long Balance { get; set; }

    /// <summary>
    /// Saldo reservado da conta
    /// </summary>
    public long ReservedBalance { get; set; }

    /// <summary>
    /// Saldo disponível para uso
    /// </summary>
    public long AvailableBalance { get; set; }

    /// <summary>
    /// Data e hora da operação (ISO 8601)
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Mensagem de erro (se aplicável)
    /// </summary>
    public string? ErrorMessage { get; set; }
}
