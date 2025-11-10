using PagueVeloz.Application.DTOs;
using PagueVeloz.Application.DTOs.Requests;
using PagueVeloz.Application.DTOs.Responses;
using PagueVeloz.Application.Interfaces;
using PagueVeloz.Domain.Entities;

namespace PagueVeloz.Application.Services;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(
        IClienteRepository clienteRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClienteDto> CriarClienteAsync(CriarClienteRequest request, CancellationToken cancellationToken = default)
    {
        if (await _clienteRepository.ExisteUsuarioAsync(request.Usuario, cancellationToken))
            throw new InvalidOperationException("Usuário já existe.");

        if (await _clienteRepository.ExisteEmailAsync(request.Email, cancellationToken))
            throw new InvalidOperationException("Email já está em uso.");

        var senhaHash = _passwordHasher.HashPassword(request.Senha);
        var cliente = new Cliente(request.Nome, request.Email, request.Usuario, senhaHash);

        await _clienteRepository.AddAsync(cliente, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Usuario = cliente.Usuario,
            AutenticacaoMultiFatorAtiva = cliente.AutenticacaoMultiFatorAtiva,
            DataCriacao = cliente.DataCriacao,
            Ativo = cliente.Ativo
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByUsuarioAsync(request.Usuario, cancellationToken);

        if (cliente == null || !cliente.Ativo)
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        if (!_passwordHasher.VerifyPassword(request.Senha, cliente.SenhaHash))
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var token = _tokenService.GenerateToken(cliente.Id, cliente.Usuario, cliente.Email);

        return new LoginResponse
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            Token = token,
            RequerAutenticacaoMultiFator = cliente.AutenticacaoMultiFatorAtiva
        };
    }
}

