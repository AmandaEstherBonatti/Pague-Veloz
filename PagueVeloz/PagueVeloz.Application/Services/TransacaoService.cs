using PagueVeloz.Application.DTOs;
using PagueVeloz.Application.DTOs.Requests;
using PagueVeloz.Application.DTOs.Responses;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Domain.Entities;
using PagueVeloz.Domain.Enums;
using PagueVeloz.Domain.Services;
using PagueVeloz.Domain.ValueObjects;

namespace PagueVeloz.Application.Services;

public class TransacaoService
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IContaRepository _contaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;
    private readonly ProcessadorTransacoesService _processadorTransacoes;
    private readonly ValidadorSaldoService _validadorSaldo;

    public TransacaoService(
        ITransacaoRepository transacaoRepository,
        IContaRepository contaRepository,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        ProcessadorTransacoesService processadorTransacoes,
        ValidadorSaldoService validadorSaldo)
    {
        _transacaoRepository = transacaoRepository;
        _contaRepository = contaRepository;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
        _processadorTransacoes = processadorTransacoes;
        _validadorSaldo = validadorSaldo;
    }

    public async Task<TransacaoResponse> ProcessarTransacaoAsync(
        ProcessarTransacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validar currency
        if (request.Currency != "BRL")
        {
            throw new InvalidOperationException($"Currency '{request.Currency}' não suportada. Apenas BRL é suportado.");
        }

        // Converter account_id string para Guid
        if (!Guid.TryParse(request.AccountId, out var contaIdGuid))
        {
            throw new InvalidOperationException($"account_id inválido: {request.AccountId}");
        }

        // Converter amount (centavos) para decimal
        var valorDecimal = request.Amount / 100.0m;

        // Extrair descrição do metadata
        var descricao = request.Metadata?.ContainsKey("description") == true
            ? request.Metadata["description"]?.ToString()
            : null;

        // Converter operation para TipoTransacao (case insensitive)
        var operationLower = request.Operation.ToLowerInvariant();
        TipoTransacao tipoTransacao;
        switch (operationLower)
        {
            case "credit":
                tipoTransacao = TipoTransacao.Credito;
                break;
            case "debit":
                tipoTransacao = TipoTransacao.Debito;
                break;
            case "reserve":
                tipoTransacao = TipoTransacao.Reserva;
                break;
            case "capture":
                tipoTransacao = TipoTransacao.Captura;
                break;
            case "reversal":
                tipoTransacao = TipoTransacao.Estorno;
                break;
            case "transfer":
                tipoTransacao = TipoTransacao.Transferencia;
                break;
            default:
                throw new InvalidOperationException($"Operação inválida: {request.Operation}. Operações válidas: credit, debit, reserve, capture, reversal, transfer");
        }

        var referenceId = new ReferenceId(request.ReferenceId);

        // Verificar idempotência
        var transacaoExistente = await _transacaoRepository.GetByReferenceIdAsync(referenceId, cancellationToken);
        if (transacaoExistente != null)
        {
            var contaExistente = await _contaRepository.GetByIdAsync(transacaoExistente.ContaId, cancellationToken);
            if (contaExistente == null)
                throw new InvalidOperationException("Conta não encontrada para transação existente.");

            return CriarTransacaoResponse(transacaoExistente, contaExistente);
        }

        // Obter conta com lock para evitar concorrência
        var conta = await _contaRepository.GetWithLockAsync(contaIdGuid, cancellationToken);
        if (conta == null)
            throw new InvalidOperationException($"Conta não encontrada: {request.AccountId}");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            Guid? contaDestinoIdGuid = null;
            if (!string.IsNullOrEmpty(request.AccountDestinationId))
            {
                if (!Guid.TryParse(request.AccountDestinationId, out var contaDestinoGuid))
                {
                    throw new InvalidOperationException($"account_destination_id inválido: {request.AccountDestinationId}");
                }
                contaDestinoIdGuid = contaDestinoGuid;
            }

            var transacao = new Transacao(
                contaIdGuid,
                referenceId,
                tipoTransacao,
                valorDecimal,
                descricao,
                contaDestinoIdGuid);

            await _transacaoRepository.AddAsync(transacao, cancellationToken);

            // Processar transação baseado no tipo
            switch (tipoTransacao)
            {
                case TipoTransacao.Credito:
                    _processadorTransacoes.ProcessarCredito(conta, transacao);
                    break;

                case TipoTransacao.Debito:
                    _processadorTransacoes.ProcessarDebito(conta, transacao);
                    break;

                case TipoTransacao.Reserva:
                    _processadorTransacoes.ProcessarReserva(conta, transacao);
                    break;

                case TipoTransacao.Captura:
                    _processadorTransacoes.ProcessarCaptura(conta, transacao);
                    break;

                case TipoTransacao.Transferencia:
                    if (!contaDestinoIdGuid.HasValue)
                        throw new InvalidOperationException("account_destination_id é obrigatório para transferência.");

                    var contaDestino = await _contaRepository.GetWithLockAsync(
                        contaDestinoIdGuid.Value,
                        cancellationToken);

                    if (contaDestino == null)
                        throw new InvalidOperationException($"Conta destino não encontrada: {request.AccountDestinationId}");

                    _processadorTransacoes.ProcessarTransferencia(conta, contaDestino, transacao);
                    await _contaRepository.UpdateAsync(contaDestino, cancellationToken);
                    break;

                case TipoTransacao.Estorno:
                    // Estorno deve ser feito via endpoint específico
                    throw new InvalidOperationException("Estorno deve ser feito via endpoint /api/transacoes/{id}/estornar");

                default:
                    throw new InvalidOperationException($"Tipo de transação inválido: {tipoTransacao}");
            }

            await _contaRepository.UpdateAsync(conta, cancellationToken);
            await _transacaoRepository.UpdateAsync(transacao, cancellationToken);

            // Publicar eventos de domínio
            if (conta.DomainEvents.Any())
            {
                await _eventBus.PublishAsync(conta.DomainEvents, cancellationToken);
                conta.LimparEventos();
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Recarregar conta para obter saldos atualizados
            conta = await _contaRepository.GetByIdAsync(contaIdGuid, cancellationToken);
            if (conta == null)
                throw new InvalidOperationException("Conta não encontrada após processamento.");

            return CriarTransacaoResponse(transacao, conta);
        }
        catch (InvalidOperationException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao processar transação: {ex.Message}", ex);
        }
    }

    private TransacaoResponse CriarTransacaoResponse(Transacao transacao, Conta conta)
    {
        // Converter decimal para centavos (long)
        var balanceCentavos = (long)(conta.CalcularSaldoTotal() * 100);
        var reservedBalanceCentavos = (long)(conta.SaldoReservado.Valor * 100);
        var availableBalanceCentavos = (long)(conta.SaldoDisponivel.Valor * 100);

        // Mapear status
        string status;
        switch (transacao.Status)
        {
            case StatusTransacao.Processada:
                status = "success";
                break;
            case StatusTransacao.Falhada:
                status = "failed";
                break;
            case StatusTransacao.Pendente:
                status = "pending";
                break;
            case StatusTransacao.Estornada:
                status = "success"; // Estorno bem-sucedido
                break;
            default:
                status = "pending";
                break;
        }

        return new TransacaoResponse
        {
            TransactionId = transacao.Id.ToString(),
            Status = status,
            Balance = balanceCentavos,
            ReservedBalance = reservedBalanceCentavos,
            AvailableBalance = availableBalanceCentavos,
            Timestamp = transacao.DataProcessamento ?? transacao.DataCriacao,
            ErrorMessage = transacao.Status == StatusTransacao.Falhada ? transacao.Erro : null
        };
    }

    public async Task<TransacaoResponse> EstornarTransacaoAsync(
        Guid transacaoId,
        string referenceIdEstorno,
        CancellationToken cancellationToken = default)
    {
        var transacaoOriginal = await _transacaoRepository.GetByIdAsync(transacaoId, cancellationToken);
        if (transacaoOriginal == null)
            throw new InvalidOperationException("Transação não encontrada.");

        if (transacaoOriginal.Status == StatusTransacao.Estornada)
            throw new InvalidOperationException("Transação já foi estornada.");

        var conta = await _contaRepository.GetWithLockAsync(transacaoOriginal.ContaId, cancellationToken);
        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var referenceId = new ReferenceId(referenceIdEstorno);
            var transacaoEstorno = new Transacao(
                transacaoOriginal.ContaId,
                referenceId,
                TipoTransacao.Estorno,
                transacaoOriginal.Valor,
                $"Estorno da transação {transacaoOriginal.Id}");

            await _transacaoRepository.AddAsync(transacaoEstorno, cancellationToken);

            _processadorTransacoes.ProcessarEstorno(conta, transacaoOriginal, transacaoEstorno);

            await _contaRepository.UpdateAsync(conta, cancellationToken);
            await _transacaoRepository.UpdateAsync(transacaoOriginal, cancellationToken);
            await _transacaoRepository.UpdateAsync(transacaoEstorno, cancellationToken);

            if (conta.DomainEvents.Any())
            {
                await _eventBus.PublishAsync(conta.DomainEvents, cancellationToken);
                conta.LimparEventos();
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Recarregar conta para obter saldos atualizados
            conta = await _contaRepository.GetByIdAsync(conta.Id, cancellationToken);
            if (conta == null)
                throw new InvalidOperationException("Conta não encontrada após estorno.");

            return CriarTransacaoResponse(transacaoEstorno, conta);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw new InvalidOperationException($"Erro ao estornar transação: {ex.Message}", ex);
        }
    }

    public async Task<ExtratoResponse> ObterExtratoAsync(
        Guid contaId,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        CancellationToken cancellationToken = default)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId, cancellationToken);
        if (conta == null)
            throw new InvalidOperationException("Conta não encontrada.");

        var inicio = dataInicio ?? DateTime.UtcNow.AddDays(-30);
        var fim = dataFim ?? DateTime.UtcNow;

        var transacoes = await _transacaoRepository.GetByContaIdAndPeriodoAsync(
            contaId,
            inicio,
            fim,
            cancellationToken);

        var transacoesDto = transacoes.Select(t => new TransacaoDto
        {
            Id = t.Id,
            ContaId = t.ContaId,
            ContaDestinoId = t.ContaDestinoId,
            ReferenceId = t.ReferenceId.Valor,
            Tipo = t.Tipo.ToString(),
            Valor = t.Valor,
            Status = t.Status.ToString(),
            Descricao = t.Descricao,
            DataCriacao = t.DataCriacao,
            DataProcessamento = t.DataProcessamento,
            Erro = t.Erro
        }).ToList();

        return new ExtratoResponse
        {
            ContaId = conta.Id,
            NumeroConta = conta.Numero,
            DataInicio = inicio,
            DataFim = fim,
            Transacoes = transacoesDto,
            TotalTransacoes = transacoesDto.Count
        };
    }
}
