using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PagueVeloz.Application.DTOs;
using PagueVeloz.Application.DTOs.Requests;
using PagueVeloz.Application.DTOs.Responses;
using PagueVeloz.Application.Services;
using PagueVeloz.Infrastructure.Metrics;

namespace PagueVeloz.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;
    private readonly ILogger<ClientesController> _logger;
    private readonly MetricsService _metricsService;

    public ClientesController(
        ClienteService clienteService,
        ILogger<ClientesController> logger,
        MetricsService metricsService)
    {
        _clienteService = clienteService;
        _logger = logger;
        _metricsService = metricsService;
    }

    [HttpPost]
    [EnableRateLimiting("authentication")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> CriarCliente([FromBody] CriarClienteRequest request)
    {
        try
        {
            var cliente = await _clienteService.CriarClienteAsync(request);
            _metricsService.IncrementarClienteCriado();
            return CreatedAtAction(nameof(ObterCliente), new { id = cliente.Id }, cliente);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao criar cliente");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [EnableRateLimiting("authentication")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _clienteService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Tentativa de login inválida");
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> ObterCliente(Guid id)
    {
        // TODO: Implementar busca de cliente
        return NotFound();
    }
}

