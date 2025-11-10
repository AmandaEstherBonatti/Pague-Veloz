using System.ComponentModel.DataAnnotations;

namespace PagueVeloz.Application.DTOs.Requests;

public class CriarContaRequest
{
    [Required(ErrorMessage = "ClienteId é obrigatório")]
    public Guid ClienteId { get; set; }

    [Required(ErrorMessage = "Limite de crédito é obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "Limite de crédito deve ser maior ou igual a zero")]
    public decimal LimiteCredito { get; set; }
}

