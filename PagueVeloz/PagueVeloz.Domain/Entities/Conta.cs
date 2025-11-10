using PagueVeloz.Domain.ValueObjects;
using PagueVeloz.Domain.Events;
using PagueVeloz.Domain.Enums;

namespace PagueVeloz.Domain.Entities;

public class Conta
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public string Numero { get; private set; }
    public Saldo SaldoDisponivel { get; private set; }
    public Saldo SaldoReservado { get; private set; }
    public LimiteCredito LimiteCredito { get; private set; }
    public StatusConta Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? UltimaAtualizacao { get; private set; }
    
    // Navigation properties
    public virtual Cliente Cliente { get; private set; } = null!;
    public virtual ICollection<Transacao> Transacoes { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Conta() { } // EF Core

    public Conta(Guid clienteId, string numero, decimal limiteCredito)
    {
        Id = Guid.NewGuid();
        ClienteId = clienteId;
        Numero = numero;
        SaldoDisponivel = new Saldo(0);
        SaldoReservado = new Saldo(0);
        LimiteCredito = new LimiteCredito(limiteCredito);
        Status = StatusConta.Ativa;
        DataCriacao = DateTime.UtcNow;
        Transacoes = new List<Transacao>();
    }

    public void AdicionarEvento(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void LimparEventos()
    {
        _domainEvents.Clear();
    }

    public void AtualizarLimiteCredito(decimal novoLimite)
    {
        LimiteCredito = new LimiteCredito(novoLimite);
        UltimaAtualizacao = DateTime.UtcNow;
    }

    public void Bloquear()
    {
        Status = StatusConta.Bloqueada;
        UltimaAtualizacao = DateTime.UtcNow;
        AdicionarEvento(new ContaBloqueadaEvent(Id, ClienteId, DateTime.UtcNow));
    }

    public void Desbloquear()
    {
        Status = StatusConta.Ativa;
        UltimaAtualizacao = DateTime.UtcNow;
        AdicionarEvento(new ContaDesbloqueadaEvent(Id, ClienteId, DateTime.UtcNow));
    }

    public void Inativar()
    {
        Status = StatusConta.Inativa;
        UltimaAtualizacao = DateTime.UtcNow;
        AdicionarEvento(new ContaInativadaEvent(Id, ClienteId, DateTime.UtcNow));
    }

    public decimal CalcularSaldoTotal()
    {
        return SaldoDisponivel.Valor + SaldoReservado.Valor;
    }

    public decimal CalcularLimiteDisponivel()
    {
        return LimiteCredito.Valor + SaldoDisponivel.Valor;
    }

    // Métodos para atualizar saldos (usados pelo domain service)
    internal void AtualizarSaldoDisponivel(Saldo novoSaldo)
    {
        SaldoDisponivel = novoSaldo;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    internal void AtualizarSaldoReservado(Saldo novoSaldo)
    {
        SaldoReservado = novoSaldo;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    internal void AtualizarSaldos(Saldo saldoDisponivel, Saldo saldoReservado)
    {
        SaldoDisponivel = saldoDisponivel;
        SaldoReservado = saldoReservado;
        UltimaAtualizacao = DateTime.UtcNow;
    }
}

