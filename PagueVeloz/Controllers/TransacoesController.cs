using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PagueVeloz.Application.DTOs.Requests;
using PagueVeloz.Application.DTOs.Responses;
using PagueVeloz.Application.Services;
using PagueVeloz.Infrastructure.Metrics;
using System.Diagnostics;

namespace PagueVeloz.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly TransacaoService _transacaoService;
    private readonly ILogger<TransacoesController> _logger;
    private readonly MetricsService _metricsService;

    public TransacoesController(
        TransacaoService transacaoService,
        ILogger<TransacoesController> logger,
        MetricsService metricsService)
    {
        _transacaoService = transacaoService;
        _logger = logger;
        _metricsService = metricsService;
    }

    [HttpPost]
    [EnableRateLimiting("transactions")]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransacaoResponse>> ProcessarTransacao([FromBody] ProcessarTransacaoRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await _transacaoService.ProcessarTransacaoAsync(request);
            stopwatch.Stop();
            _metricsService.IncrementarTransacaoProcessada(request.Operation);
            _metricsService.RegistrarTempoProcessamento(stopwatch.Elapsed.TotalSeconds, request.Operation);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao processar transação");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metricsService.IncrementarTransacaoFalhada(request.Operation, ex.Message);
            _logger.LogError(ex, "Erro inesperado ao processar transação");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpPost("{id}/estornar")]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransacaoResponse>> EstornarTransacao(
        Guid id,
        [FromBody] EstornarTransacaoRequest request)
    {
        try
        {
            var response = await _transacaoService.EstornarTransacaoAsync(id, request.ReferenceId);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao estornar transação");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao estornar transação");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("conta/{contaId}/extrato")]
    [ProducesResponseType(typeof(ExtratoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExtratoResponse>> ObterExtrato(
        Guid contaId,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        try
        {
            var extrato = await _transacaoService.ObterExtratoAsync(contaId, dataInicio, dataFim);
            return Ok(extrato);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao obter extrato");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao obter extrato");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}

public class EstornarTransacaoRequest
{
    public string ReferenceId { get; set; } = string.Empty;
}

