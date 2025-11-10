using System.ComponentModel.DataAnnotations;

namespace PagueVeloz.Application.DTOs.Requests;

/// <summary>
/// Request para processar transação financeira conforme especificação do desafio
/// </summary>
public class ProcessarTransacaoRequest
{
    /// <summary>
    /// Tipo de operação: credit, debit, reserve, capture, reversal, transfer
    /// </summary>
    [Required(ErrorMessage = "operation é obrigatório")]
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Identificador único da conta
    /// </summary>
    [Required(ErrorMessage = "account_id é obrigatório")]
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// Valor da operação em centavos (inteiro)
    /// </summary>
    [Required(ErrorMessage = "amount é obrigatório")]
    [Range(1, long.MaxValue, ErrorMessage = "amount deve ser maior que zero")]
    public long Amount { get; set; }

    /// <summary>
    /// Moeda da operação (ex: "BRL")
    /// </summary>
    [Required(ErrorMessage = "currency é obrigatório")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "currency deve ter 3 caracteres")]
    public string Currency { get; set; } = "BRL";

    /// <summary>
    /// Identificador único da transação para idempotência
    /// </summary>
    [Required(ErrorMessage = "reference_id é obrigatório")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "reference_id deve ter entre 1 e 100 caracteres")]
    public string ReferenceId { get; set; } = string.Empty;

    /// <summary>
    /// Dados adicionais opcionais
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Conta destino (apenas para transferências)
    /// </summary>
    public string? AccountDestinationId { get; set; }
}
