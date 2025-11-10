# Guia de Testes - PagueVeloz

Este documento descreve como executar e escrever testes para o projeto PagueVeloz.

## Estrutura de Testes

O projeto possui três projetos de teste:

1. **PagueVeloz.Domain.Tests**: Testes unitários para a camada de domínio
2. **PagueVeloz.Application.Tests**: Testes unitários para serviços de aplicação
3. **PagueVeloz.Integration.Tests**: Testes de integração end-to-end

## Executando os Testes

### Executar todos os testes

```bash
dotnet test
```

### Executar testes de um projeto específico

```bash
dotnet test PagueVeloz.Domain.Tests
dotnet test PagueVeloz.Application.Tests
dotnet test PagueVeloz.Integration.Tests
```

### Executar com cobertura de código

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Escrevendo Testes

### Testes Unitários - Domain

Testes para Value Objects, Entidades e Domain Services:

```csharp
[Fact]
public void CriarSaldo_ComValorValido_DeveCriarComSucesso()
{
    // Arrange & Act
    var saldo = new Saldo(100.50m);

    // Assert
    Assert.Equal(100.50m, saldo.Valor);
}
```

### Testes Unitários - Application

Testes para Services usando Moq para mocks:

```csharp
[Fact]
public async Task CriarClienteAsync_ComDadosValidos_DeveCriarCliente()
{
    // Arrange
    var request = new CriarClienteRequest { ... };
    _clienteRepositoryMock.Setup(...);

    // Act
    var result = await _clienteService.CriarClienteAsync(request);

    // Assert
    Assert.NotNull(result);
    _clienteRepositoryMock.Verify(...);
}
```

### Testes de Integração

Testes que executam a aplicação completa:

```csharp
[Fact]
public async Task Post_CriarCliente_DeveRetornarCreated()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new { ... };

    // Act
    var response = await client.PostAsJsonAsync("/api/clientes", request);

    // Assert
    response.EnsureSuccessStatusCode();
}
```

## Boas Práticas

1. **Nomenclatura**: Use nomes descritivos que expliquem o que está sendo testado
2. **AAA Pattern**: Arrange, Act, Assert
3. **Isolamento**: Cada teste deve ser independente
4. **Cobertura**: Busque cobertura de pelo menos 70% para código crítico
5. **Mocks**: Use mocks apenas quando necessário (testes de integração preferem dados reais)

## Pacotes Utilizados

- **xUnit**: Framework de testes
- **Moq**: Biblioteca de mocking
- **FluentAssertions**: Asserções mais legíveis
- **Microsoft.AspNetCore.Mvc.Testing**: Para testes de integração

