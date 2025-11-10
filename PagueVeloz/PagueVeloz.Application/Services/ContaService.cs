using PagueVeloz.Application.DTOs;
using PagueVeloz.Application.DTOs.Requests;
using PagueVeloz.Application.DTOs.Responses;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Application.Services;

public class ContaService
{
    private readonly IContaRepository _contaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ContaService(
        IContaRepository contaRepository,
        IClienteRepository clienteRepository,
        IUnitOfWork unitOfWork)
    {
        _contaRepository = contaRepository;
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ContaDto> CriarContaAsync(CriarContaRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId, cancellationToken);

        if (cliente == null)
            throw new InvalidOperationException("Cliente não encontrado.");

        if (!cliente.Ativo)
            throw new InvalidOperationException("Cliente inativo não pode criar contas.");

        var numeroConta = GerarNumeroConta();
        var conta = new Conta(request.ClienteId, numeroConta, request.LimiteCredito);

        await _contaRepository.AddAsync(conta, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ContaDto
        {
            Id = conta.Id,
            ClienteId = conta.ClienteId,
            Numero = conta.Numero,
            SaldoDisponivel = conta.SaldoDisponivel.Valor,
            SaldoReservado = conta.SaldoReservado.Valor,
            SaldoTotal = conta.CalcularSaldoTotal(),
            LimiteCredito = conta.LimiteCredito.Valor,
            Status = conta.Status.ToString(),
            DataCriacao = conta.DataCriacao
        };
    }

    public async Task<SaldoResponse> ConsultarSaldoAsync(Guid contaId, CancellationToken cancellationToken = default)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId, cancellationToken);

        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada.");

        return new SaldoResponse
        {
            ContaId = conta.Id,
            NumeroConta = conta.Numero,
            SaldoDisponivel = conta.SaldoDisponivel.Valor,
            SaldoReservado = conta.SaldoReservado.Valor,
            SaldoTotal = conta.CalcularSaldoTotal(),
            LimiteCredito = conta.LimiteCredito.Valor,
            LimiteDisponivel = conta.CalcularLimiteDisponivel()
        };
    }

    public async Task<ContaDto> ObterContaPorIdAsync(Guid contaId, CancellationToken cancellationToken = default)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId, cancellationToken);

        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada.");

        return new ContaDto
        {
            Id = conta.Id,
            ClienteId = conta.ClienteId,
            Numero = conta.Numero,
            SaldoDisponivel = conta.SaldoDisponivel.Valor,
            SaldoReservado = conta.SaldoReservado.Valor,
            SaldoTotal = conta.CalcularSaldoTotal(),
            LimiteCredito = conta.LimiteCredito.Valor,
            Status = conta.Status.ToString(),
            DataCriacao = conta.DataCriacao
        };
    }

    public async Task<IEnumerable<ContaDto>> ObterContasPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        var contas = await _contaRepository.GetByClienteIdAsync(clienteId, cancellationToken);

        return contas
            .OrderByDescending(c => c.DataCriacao)
            .Select(c => new ContaDto
            {
                Id = c.Id,
                ClienteId = c.ClienteId,
                Numero = c.Numero,
                SaldoDisponivel = c.SaldoDisponivel.Valor,
                SaldoReservado = c.SaldoReservado.Valor,
                SaldoTotal = c.CalcularSaldoTotal(),
                LimiteCredito = c.LimiteCredito.Valor,
                Status = c.Status.ToString(),
                DataCriacao = c.DataCriacao
            });
    }

    private static string GerarNumeroConta()
    {
        var random = new Random();
        return $"{random.Next(1000, 9999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
    }
}

