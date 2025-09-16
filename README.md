# 🏦 CleanCode --- Sistema de Controle de Fluxo de Caixa - Desafio Carrefour

## 📋 Visão Geral

API RESTful em **.NET 9** para **gestão completa de fluxo de caixa**,
desenvolvida com **Clean Architecture** e princípios **SOLID**.

Funcionalidades principais: - 🔐 **Autenticação JWT** com Bearer token
flexível\
- 👥 **Gestão de usuários**  
- 💰 **Controle de transações** (débitos e créditos)\
- 📊 **Consolidação automática de saldo diário**\
- 📨 **Mensageria** baseada em filas (file-based) para testes e evolução
futura\
- 🧪 **Testes automatizados**  

------------------------------------------------------------------------

## 🏗️ Arquitetura

### Estrutura de Camadas

    CleanCode/
    ├── CleanCode.Api/         # Apresentação (Controllers, Swagger, Middlewares)
    ├── CleanCode.Application/ # Casos de uso (CQRS, Handlers, Validations)
    ├── CleanCode.Domain/      # Entidades e regras de negócio
    ├── CleanCode.ORM/         # Acesso a dados (EF Core, Repositories, Migrations)
    ├── CleanCode.Common/      # Infra compartilhada (Security, Logging, Messaging)
    ├── CleanCode.IoC/         # Injeção de dependência
    └── CleanCode.Tests/       # Testes unitários e de integração

### Padrões e Princípios-Chave

-   **Clean Architecture**: domínio independente de frameworks;
    dependências fluem sempre para dentro.\
-   **CQRS + MediatR**: separação de comandos (escrita) e queries
    (leitura).\
-   **Repository Pattern**: abstração de acesso a dados.\
-   **Specification Pattern**: regras de consulta reutilizáveis e
    compostas.\
-   **Pipeline Behavior**: validação e logging centralizados.\
-   **Mensageria assíncrona**: filas em disco, permitindo simulação de
    eventos e preparando para integrações reais (ex.: RabbitMQ, Azure
    Service Bus).

------------------------------------------------------------------------

## 🔧 Stack e Ferramentas

-   **.NET 9**, **Entity Framework Core**, **SQL Server**\
-   **MediatR**, **AutoMapper**, **FluentValidation**\
-   **JWT + BCrypt** para autenticação e segurança\
-   **Serilog** para logging estruturado e health checks\
-   **xUnit**, **Moq**, **Bogus**, **FluentAssertions** para testes\
-   **Swagger** com autenticação integrada

------------------------------------------------------------------------

## 🔑 Segurança e Observabilidade

-   **JWT Bearer flexível** (aceita token com ou sem prefixo Bearer)\
-   Hash de senhas seguro com **BCrypt**\
-   Middleware global para tratamento de exceções\
-   **Health checks** para banco, mensageria e dependências\
-   Logging otimizado com rotação diária e retenção de 7 dias

------------------------------------------------------------------------

## 🚀 Fluxo de Dados

    Request → Controller → MediatR → Handler → Repository → Banco
                    ↓
    Response ← DTO ← Mapper ← Result ← Entity ← ORM
                    ↓
    Message → MessageService → Queue (File System)

> As filas criadas em disco servem como **mecanismo de mensageria** para
> testes e prototipagem, facilitando a evolução para uma solução real de
> mensageria (RabbitMQ, Kafka ou Azure Service Bus) sem alterar a lógica
> de domínio.

------------------------------------------------------------------------

## 🧪 Testes

-   **111 testes passando** (unitários, integração e domínio)\
-   Cobertura total de casos de uso, controllers e serviços de
    infraestrutura\
-   Ferramentas: **xUnit**, **Moq**, **FluentAssertions**, **Bogus**

Execução:

``` bash
dotnet test CleanCode.Tests/CleanCode.Tests.csproj
```

------------------------------------------------------------------------

## ▶️ Passo a passo oficial (SQL Server + EF Core Migrations)

### 0) Pré-requisitos

-   **.NET 9 SDK**
-   **SQL Server** (local, remoto ou Docker)
-   **EF Core CLI** (se for usar a linha de comando):

``` bash
dotnet tool install -g dotnet-ef
```

### 1) Criar o Banco no SQL Server

**Opção A --- Usando SQL Server Management Studio (SSMS)**

``` sql
IF DB_ID('CleanCodeDb') IS NULL
BEGIN
    CREATE DATABASE CleanCodeDb;
END
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'cleancode_login')
BEGIN
    CREATE LOGIN cleancode_login WITH PASSWORD = 'Str0ngP@ss!';
END
GO

USE CleanCodeDb;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'cleancode_user')
BEGIN
    CREATE USER cleancode_user FOR LOGIN cleancode_login;
    EXEC sp_addrolemember N'db_owner', N'cleancode_user';
END
GO
```

**Opção B --- Via sqlcmd**

``` bash
sqlcmd -S localhost,1433 -U sa -P Strong!Passw0rd -Q "IF DB_ID('CleanCodeDb') IS NULL CREATE DATABASE CleanCodeDb"
```

**Opção C --- Docker (SQL Server container)**

``` bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Strong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

Depois, execute os scripts de criação de banco (SSMS ou sqlcmd).

------------------------------------------------------------------------

### 2) Configurar Connection String

Arquivo: `CleanCode.Api/appsettings.Development.json`

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CleanCodeDb;User Id=sa;Password=Strong!Passw0rd;TrustServerCertificate=True;"
  }
}
```

------------------------------------------------------------------------

### 3) Aplicar Migrations

**Via Package Manager Console (Visual Studio)**\
Abra `Tools → NuGet Package Manager → Package Manager Console` e
defina: - Startup Project: `CleanCode.Api` - Default project:
`CleanCode.ORM`

Rodar:

``` powershell
Update-Database -StartupProject CleanCode.Api -Project CleanCode.ORM
```

Se ainda não houver migrations:

``` powershell
Add-Migration InitialCreate -Project CleanCode.ORM -StartupProject CleanCode.Api
Update-Database -StartupProject CleanCode.Api -Project CleanCode.ORM
```

**Via CLI (dotnet ef)**\
Alternativa em linha de comando:

``` bash
dotnet ef database update --project CleanCode.ORM --startup-project CleanCode.Api
```

Para criar e aplicar a primeira migration:

``` bash
dotnet ef migrations add InitialCreate --project CleanCode.ORM --startup-project CleanCode.Api
dotnet ef database update --project CleanCode.ORM --startup-project CleanCode.Api
```

------------------------------------------------------------------------

### 4) Executar a API

``` bash
dotnet dev-certs https --trust   # apenas no primeiro uso
dotnet run --project CleanCode.Api
```

Acesse o Swagger na URL exibida no console (ex.:
`https://localhost:7000/swagger`).

------------------------------------------------------------------------

### 5) Criar usuário inicial e autenticar

``` bash
curl -X POST "https://localhost:7000/api/users"   -H "Content-Type: application/json"   -d '{ "name": "Admin", "email": "admin@admin.com", "password": "Admin@123", "role": "Admin" }'

curl -X POST "https://localhost:7000/api/auth"   -H "Content-Type: application/json"   -d '{ "email": "admin@admin.com", "password": "Admin@123" }'
```

Copie o `token` da resposta e use em **Authorize** no Swagger (com ou
sem prefixo `Bearer`).

------------------------------------------------------------------------

### 6) Testar fluxo mínimo

1.  Criar transação\
2.  Listar transações\
3.  Consolidar saldo diário\
4.  Consultar saldo consolidado

------------------------------------------------------------------------

## 💡 Benefícios Arquiteturais

-   **Manutenibilidade**: código desacoplado e organizado\
-   **Escalabilidade**: pronto para crescimento e novas features\
-   **Flexibilidade**: troca de implementações sem impacto no domínio\
-   **Segurança e Resiliência**: autenticação robusta e health checks\
-   **Alta Testabilidade**: camadas isoladas facilitam cobertura
    automatizada

> As filas em disco já foram projetadas para **testes e futuras
> integrações reais de mensageria**, suportando cenários de event
> sourcing, notificações em tempo real ou integrações com outros
> microsserviços.

------------------------------------------------------------------------

## 🔮 Próximos Passos como Arquiteto

Como evolução natural, o próximo roadmap arquitetural incluiria: -
**Substituir o mecanismo de filas em disco** por uma mensageria de
produção (ex.: RabbitMQ, Azure Service Bus ou Kafka).\
- **Escalar leitura/escrita** com **CQRS avançado** e **event
sourcing**.\
- **Introduzir microsserviços** para módulos como usuários e
consolidação financeira.\
- **Observabilidade completa** com OpenTelemetry, métricas de
performance e tracing distribuído.\
- **Pipeline CI/CD** com testes de mutação, análise estática e deploy
automatizado em nuvem (Azure ou AWS).\
- **Hardening de segurança**: secrets em Key Vault, políticas de rotação
de chaves, autenticação multi-fator.

------------------------------------------------------------------------

## 📌 Conclusão

O **CleanCode** demonstra domínio em **arquitetura moderna** e padrões
consagrados: - **Clean Architecture + CQRS + SOLID** - **Repository e
Specification Pattern** - **Mensageria assíncrona e logging
estruturado**

Resultado: um sistema **robusto, escalável e totalmente testado**,
pronto para produção e preparado para evoluir com segurança e alta
disponibilidade.
