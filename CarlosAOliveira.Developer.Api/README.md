# Carrefour API

Uma API sofisticada e elegante para gerenciamento de merchants e transações, desenvolvida seguindo as melhores práticas de arquitetura de software.

## 🏗️ Arquitetura

A API foi desenvolvida seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**:

- **Domain Layer**: Entidades, enums, eventos e validações de domínio
- **Application Layer**: Casos de uso, handlers, DTOs e validações
- **Infrastructure Layer**: Repositórios, Entity Framework e configurações
- **API Layer**: Controllers, middleware, autenticação e documentação

## 🚀 Tecnologias

- **.NET 9.0**
- **Entity Framework Core** com SQL Server
- **JWT Authentication** com refresh tokens
- **MediatR** para CQRS
- **AutoMapper** para mapeamento de objetos
- **FluentValidation** para validações
- **Serilog** para logging estruturado
- **Swagger/OpenAPI** para documentação
- **xUnit** para testes unitários

## 🔧 Configuração

### Pré-requisitos

- .NET 9.0 SDK
- SQL Server (servidor: NOTBOOK, usuário: sa, senha: 123)
- Visual Studio 2022 ou VS Code

### Configuração do Banco de Dados

A string de conexão está configurada no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=NOTBOOK;Database=carrefour;User Id=sa;Password=123;TrustServerCertificate=true;MultipleActiveResultSets=true;"
  }
}
```

### Executando a API

1. Clone o repositório
2. Restaure as dependências:
   ```bash
   dotnet restore
   ```
3. Execute a API:
   ```bash
   dotnet run --project CarlosAOliveira.Developer.Api
   ```
4. Acesse a documentação Swagger em: `https://localhost:7000`

## 🔐 Autenticação

A API utiliza JWT (JSON Web Tokens) para autenticação:

### Endpoints de Autenticação

- `POST /api/auth/login` - Login do usuário
- `POST /api/auth/refresh` - Renovar token de acesso
- `GET /api/auth/me` - Informações do usuário atual

### Configuração JWT

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "CarrefourAPI",
    "Audience": "CarrefourUsers",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

## 📚 Endpoints da API

### Merchants

- `GET /api/merchants` - Listar merchants (paginado)
- `GET /api/merchants/{id}` - Obter merchant por ID
- `POST /api/merchants` - Criar novo merchant
- `PUT /api/merchants/{id}` - Atualizar merchant
- `DELETE /api/merchants/{id}` - Deletar merchant

### Transactions

- `GET /api/transactions/{id}` - Obter transação por ID
- `GET /api/transactions/merchant/{merchantId}` - Listar transações por merchant
- `POST /api/transactions` - Criar nova transação

### Daily Summaries

- `GET /api/dailysummaries/{id}` - Obter resumo diário por ID
- `GET /api/dailysummaries/merchant/{merchantId}/period` - Resumo por período

## 🧪 Testes

Execute todos os testes:

```bash
dotnet test
```

Execute testes específicos:

```bash
dotnet test CarlosAOliveira.Developer.Tests
```

## 📊 Logging

A API utiliza Serilog para logging estruturado:

- **Console**: Logs em tempo real durante desenvolvimento
- **File**: Logs salvos em `logs/carrefour-api-{date}.log`
- **Request Logging**: Log automático de todas as requisições HTTP

## 🛡️ Segurança

- **JWT Authentication**: Tokens seguros com expiração
- **CORS**: Configurado para permitir requisições cross-origin
- **Global Exception Handling**: Tratamento centralizado de exceções
- **Input Validation**: Validação de entrada em todos os endpoints
- **SQL Injection Protection**: Entity Framework Core com parâmetros

## 🔄 Padrões Implementados

- **Repository Pattern**: Abstração de acesso a dados
- **CQRS**: Separação de comandos e consultas
- **Mediator Pattern**: Desacoplamento de handlers
- **Dependency Injection**: Inversão de controle
- **Builder Pattern**: Criação de objetos de teste
- **Factory Pattern**: Criação de contextos de banco

## 📈 Monitoramento

- **Health Checks**: Endpoint `/health` para verificação de saúde
- **Structured Logging**: Logs estruturados para análise
- **Exception Tracking**: Rastreamento de exceções com contexto

## 🎯 Próximos Passos

- [ ] Implementar cache com Redis
- [ ] Adicionar métricas com Application Insights
- [ ] Implementar rate limiting
- [ ] Adicionar testes de integração
- [ ] Implementar background jobs
- [ ] Adicionar documentação de API com XML comments

## 👨‍💻 Desenvolvedor

**Carlos Oliveira** - Desenvolvedor Full Stack

---

*Esta API foi desenvolvida seguindo as melhores práticas de arquitetura de software, garantindo código limpo, testável e escalável.*
