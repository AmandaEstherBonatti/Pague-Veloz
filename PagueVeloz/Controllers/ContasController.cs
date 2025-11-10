using Microsoft.AspNetCore.Mvc;
using PagueVeloz.Application.DTOs;
using PagueVeloz.Application.DTOs.Requests;
using PagueVeloz.Application.DTOs.Responses;
using PagueVeloz.Application.Services;

namespace PagueVeloz.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContasController : ControllerBase
{
    private readonly ContaService _contaService;
    private readonly ILogger<ContasController> _logger;

    public ContasController(ContaService contaService, ILogger<ContasController> logger)
    {
        _contaService = contaService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContaDto>> CriarConta([FromBody] CriarContaRequest request)
    {
        try
        {
            var conta = await _contaService.CriarContaAsync(request);
            return CreatedAtAction(nameof(ObterConta), new { id = conta.Id }, conta);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao criar conta");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContaDto>> ObterConta(Guid id)
    {
        try
        {
            var conta = await _contaService.ObterContaPorIdAsync(id);
            return Ok(conta);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conta não encontrada");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter conta {ContaId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<ContaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ContaDto>>> ObterContasPorCliente(Guid clienteId)
    {
        try
        {
            var contas = await _contaService.ObterContasPorClienteAsync(clienteId);
            return Ok(contas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter contas do cliente");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("{id}/saldo")]
    [ProducesResponseType(typeof(SaldoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaldoResponse>> ConsultarSaldo(Guid id)
    {
        try
        {
            var saldo = await _contaService.ConsultarSaldoAsync(id);
            return Ok(saldo);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conta não encontrada");
            return NotFound(new { message = ex.Message });
        }
    }
}

