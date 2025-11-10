using PagueVeloz.Domain.ValueObjects;
using Xunit;

namespace PagueVeloz.Domain.Tests.ValueObjects;

public class SaldoTests
{
    [Fact]
    public void CriarSaldo_ComValorValido_DeveCriarComSucesso()
    {
        // Arrange & Act
        var saldo = new Saldo(100.50m);

        // Assert
        Assert.Equal(100.50m, saldo.Valor);
    }

    [Fact]
    public void CriarSaldo_ComValorNegativo_DeveLancarExcecao()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Saldo(-10m));
    }

    [Fact]
    public void Adicionar_ComValorValido_DeveRetornarNovoSaldo()
    {
        // Arrange
        var saldo = new Saldo(100m);

        // Act
        var novoSaldo = saldo.Adicionar(50m);

        // Assert
        Assert.Equal(150m, novoSaldo.Valor);
    }

    [Fact]
    public void Subtrair_ComValorValido_DeveRetornarNovoSaldo()
    {
        // Arrange
        var saldo = new Saldo(100m);

        // Act
        var novoSaldo = saldo.Subtrair(30m);

        // Assert
        Assert.Equal(70m, novoSaldo.Valor);
    }

    [Fact]
    public void Subtrair_ComValorMaiorQueSaldo_DeveLancarExcecao()
    {
        // Arrange
        var saldo = new Saldo(100m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => saldo.Subtrair(150m));
    }

    [Fact]
    public void PodeSubtrair_ComSaldoSuficiente_DeveRetornarTrue()
    {
        // Arrange
        var saldo = new Saldo(100m);

        // Act
        var podeSubtrair = saldo.PodeSubtrair(50m);

        // Assert
        Assert.True(podeSubtrair);
    }

    [Fact]
    public void PodeSubtrair_ComSaldoInsuficiente_DeveRetornarFalse()
    {
        // Arrange
        var saldo = new Saldo(100m);

        // Act
        var podeSubtrair = saldo.PodeSubtrair(150m);

        // Assert
        Assert.False(podeSubtrair);
    }
}

