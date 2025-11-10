using Moq;
using PagueVeloz.Application.DTOs.Requests;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Application.Services;
using PagueVeloz.Domain.Entities;
using Xunit;

namespace PagueVeloz.Application.Tests.Services;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly ClienteService _clienteService;

    public ClienteServiceTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _clienteService = new ClienteService(
            _clienteRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task CriarClienteAsync_ComDadosValidos_DeveCriarCliente()
    {
        // Arrange
        var request = new CriarClienteRequest
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Usuario = "joao",
            Senha = "senha123"
        };

        _clienteRepositoryMock
            .Setup(x => x.ExisteUsuarioAsync(request.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _clienteRepositoryMock
            .Setup(x => x.ExisteEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.HashPassword(request.Senha))
            .Returns("hashed_password");

        _clienteRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente c, CancellationToken ct) => c);

        // Act
        var result = await _clienteService.CriarClienteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Nome, result.Nome);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Usuario, result.Usuario);
        _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarClienteAsync_ComUsuarioExistente_DeveLancarExcecao()
    {
        // Arrange
        var request = new CriarClienteRequest
        {
            Nome = "João Silva",
            Email = "joao@example.com",
            Usuario = "joao",
            Senha = "senha123"
        };

        _clienteRepositoryMock
            .Setup(x => x.ExisteUsuarioAsync(request.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _clienteService.CriarClienteAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ComCredenciaisValidas_DeveRetornarToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Usuario = "joao",
            Senha = "senha123"
        };

        var cliente = new Cliente("João Silva", "joao@example.com", "joao", "hashed_password");

        _clienteRepositoryMock
            .Setup(x => x.GetByUsuarioAsync(request.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(request.Senha, cliente.SenhaHash))
            .Returns(true);

        _tokenServiceMock
            .Setup(x => x.GenerateToken(cliente.Id, cliente.Usuario, cliente.Email))
            .Returns("mock_token");

        // Act
        var result = await _clienteService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mock_token", result.Token);
        Assert.Equal(cliente.Id, result.ClienteId);
    }

    [Fact]
    public async Task LoginAsync_ComSenhaInvalida_DeveLancarExcecao()
    {
        // Arrange
        var request = new LoginRequest
        {
            Usuario = "joao",
            Senha = "senha_errada"
        };

        var cliente = new Cliente("João Silva", "joao@example.com", "joao", "hashed_password");

        _clienteRepositoryMock
            .Setup(x => x.GetByUsuarioAsync(request.Usuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(request.Senha, cliente.SenhaHash))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _clienteService.LoginAsync(request));
    }
}

