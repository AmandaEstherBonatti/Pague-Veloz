namespace PagueVeloz.Domain.Entities;

public class Cliente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Usuario { get; private set; }
    public string SenhaHash { get; private set; }
    public bool AutenticacaoMultiFatorAtiva { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? UltimaAtualizacao { get; private set; }
    public bool Ativo { get; private set; }
    
    // Navigation property
    public virtual ICollection<Conta> Contas { get; private set; }

    private Cliente() { } // EF Core

    public Cliente(string nome, string email, string usuario, string senhaHash)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        Usuario = usuario;
        SenhaHash = senhaHash;
        AutenticacaoMultiFatorAtiva = false;
        DataCriacao = DateTime.UtcNow;
        Ativo = true;
        Contas = new List<Conta>();
    }

    public void AtualizarPerfil(string nome, string email)
    {
        Nome = nome;
        Email = email;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarSenha(string novaSenhaHash)
    {
        SenhaHash = novaSenhaHash;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    public void AtivarAutenticacaoMultiFator()
    {
        AutenticacaoMultiFatorAtiva = true;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    public void DesativarAutenticacaoMultiFator()
    {
        AutenticacaoMultiFatorAtiva = false;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        UltimaAtualizacao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        UltimaAtualizacao = DateTime.UtcNow;
    }
}

