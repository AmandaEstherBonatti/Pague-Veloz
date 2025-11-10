using Microsoft.EntityFrameworkCore;
using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Conta> Contas { get; set; } = null!;
    public DbSet<Transacao> Transacoes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Usuario).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Usuario).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(e => e.Contas).WithOne(e => e.Cliente).HasForeignKey(e => e.ClienteId);
        });

        // Configuração Conta
        modelBuilder.Entity<Conta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Numero).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ClienteId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.HasIndex(e => e.Numero).IsUnique();
            entity.OwnsOne(e => e.SaldoDisponivel, saldo =>
            {
                saldo.Property(s => s.Valor).HasColumnName("SaldoDisponivel").IsRequired().HasPrecision(18, 2);
            });
            entity.OwnsOne(e => e.SaldoReservado, saldo =>
            {
                saldo.Property(s => s.Valor).HasColumnName("SaldoReservado").IsRequired().HasPrecision(18, 2);
            });
            entity.OwnsOne(e => e.LimiteCredito, limite =>
            {
                limite.Property(l => l.Valor).HasColumnName("LimiteCredito").IsRequired().HasPrecision(18, 2);
            });
            entity.HasMany(e => e.Transacoes).WithOne(e => e.Conta).HasForeignKey(e => e.ContaId);
        });

        // Configuração Transacao
        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContaId).IsRequired();
            entity.Property(e => e.Tipo).IsRequired();
            entity.Property(e => e.Valor).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired();
            entity.OwnsOne(e => e.ReferenceId, refId =>
            {
                refId.Property(r => r.Valor)
                    .HasColumnName("ReferenceId")
                    .IsRequired()
                    .HasMaxLength(100);
            });
            entity.HasOne(e => e.ContaDestino).WithMany().HasForeignKey(e => e.ContaDestinoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}

