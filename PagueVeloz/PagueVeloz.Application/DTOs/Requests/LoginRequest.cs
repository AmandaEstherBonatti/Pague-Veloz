using System.ComponentModel.DataAnnotations;

namespace PagueVeloz.Application.DTOs.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Usuário é obrigatório")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    public string Senha { get; set; } = string.Empty;
}

