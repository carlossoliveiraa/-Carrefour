# 🏦 CleanCode -- Sistema de Controle de Fluxo de Caixa

## 📋 Visão Geral

API RESTful em **.NET 9** para **gestão completa de fluxo de caixa**,
desenvolvida com **Clean Architecture** e princípios **SOLID**.

Funcionalidades principais: - 🔐 **Autenticação JWT** com Bearer token
flexível\
- 👥 **Gestão de usuários** com roles e status\
- 💰 **Controle de transações** (débitos e créditos)\
- 📊 **Consolidação automática** de saldo diário\
- 📨 **Mensageria** baseada em filas (file-based)\
- 🧪 **Testes automatizados** (111 testes unitários e de integração)

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
-   **CQRS + MediatR**: comandos (escrita) e queries (leitura)
    separados.\
-   **Repository Pattern**: abstração de acesso a dados.\
-   **Specification Pattern**: regras de consulta reutilizáveis.\
-   **Pipeline Behavior**: validação e logging centralizados.\
-   **Mensageria assíncrona**: filas em sistema de arquivos.

## 🔧 Stack e Ferramentas

-   **.NET 9**, **Entity Framework Core**, **SQL Server**\
-   **MediatR**, **AutoMapper**, **FluentValidation**\
-   **JWT + BCrypt** para autenticação e segurança\
-   **Serilog** para logging estruturado e health checks\
-   **xUnit**, **Moq**, **Bogus**, **FluentAssertions** para testes\
-   **Swagger** com autenticação integrada

## 🔑 Segurança e Observabilidade

-   **JWT Bearer flexível** (aceita token com ou sem prefixo Bearer)\
-   Hash de senhas seguro com **BCrypt**\
-   Middleware global para tratamento de exceções\
-   **Health checks** para banco, mensageria e dependências\
-   Logging otimizado com rotação diária e retenção de 7 dias

## 🚀 Fluxo de Dados

    Request → Controller → MediatR → Handler → Repository → Banco
                    ↓
    Response ← DTO ← Mapper ← Result ← Entity ← ORM
                    ↓
    Message → MessageService → Queue (File System)

## 🧪 Testes

-   **111 testes passando** (unitários, de integração e de domínio)\
-   Cobertura total de casos de uso, controllers e serviços de
    infraestrutura\
-   Ferramentas: **xUnit**, **Moq**, **FluentAssertions**, **Bogus**

## 💡 Benefícios Arquiteturais

-   **Manutenibilidade**: código desacoplado e organizado\
-   **Escalabilidade**: pronto para crescimento e novas features\
-   **Flexibilidade**: troca de implementações sem impacto no domínio\
-   **Segurança e Resiliência**: autenticação robusta e health checks\
-   **Alta Testabilidade**: camadas isoladas facilitam cobertura
    automatizada

## 📌 Conclusão

O **CleanCode** demonstra domínio em **arquitetura moderna** e padrões
consagrados:

-   **Clean Architecture + CQRS + SOLID**\
-   **Repository e Specification Pattern**\
-   **Mensageria assíncrona e logging estruturado**

Resultado: um sistema **robusto, escalável e totalmente testado**,
pronto para produção e evolução contínua.
