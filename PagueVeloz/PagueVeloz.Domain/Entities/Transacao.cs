using PagueVeloz.Domain.ValueObjects;
using PagueVeloz.Domain.Events;
using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Domain.Entities;

public class Transacao
{
    public Guid Id { get; private set; }
    public Guid ContaId { get; private set; }
    public Guid? ContaDestinoId { get; private set; }
    public ReferenceId ReferenceId { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public StatusTransacao Status { get; private set; }
    public string? Descricao { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataProcessamento { get; private set; }
    public string? Erro { get; private set; }
    
    // Navigation properties
    public virtual Conta Conta { get; private set; } = null!;
    public virtual Conta? ContaDestino { get; private set; }

    private Transacao() { } // EF Core

    public Transacao(
        Guid contaId,
        ReferenceId referenceId,
        TipoTransacao tipo,
        decimal valor,
        string? descricao = null,
        Guid? contaDestinoId = null)
    {
        Id = Guid.NewGuid();
        ContaId = contaId;
        ContaDestinoId = contaDestinoId;
        ReferenceId = referenceId;
        Tipo = tipo;
        Valor = valor;
        Status = StatusTransacao.Pendente;
        Descricao = descricao;
        DataCriacao = DateTime.UtcNow;
    }

    public void ProcessarComSucesso()
    {
        Status = StatusTransacao.Processada;
        DataProcessamento = DateTime.UtcNow;
    }

    public void Falhar(string erro)
    {
        Status = StatusTransacao.Falhada;
        Erro = erro;
        DataProcessamento = DateTime.UtcNow;
    }

    public void Estornar()
    {
        Status = StatusTransacao.Estornada;
        DataProcessamento = DateTime.UtcNow;
    }
}

