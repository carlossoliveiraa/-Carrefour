🏦 CleanCode – Sistema de Controle de Fluxo de Caixa
📋 Visão Geral

API RESTful em .NET 9 para gestão completa de fluxo de caixa, desenvolvida com Clean Architecture e princípios SOLID.
Funcionalidades principais:

🔐 Autenticação JWT com Bearer token flexível

👥 Gestão de usuários com roles e status

💰 Controle de transações (débitos e créditos)

📊 Consolidação automática de saldo diário

📨 Mensageria baseada em filas (file-based)

🧪 Testes automatizados (111 testes entre unitários e integração)

🏗️ Arquitetura
Estrutura de Camadas
CleanCode/
├── Api/           # Apresentação (Controllers, Swagger, Middlewares)
├── Application/   # Casos de uso (CQRS, Handlers, Validations)
├── Domain/        # Entidades e regras de negócio
├── ORM/           # Acesso a dados (EF Core, Repositories, Migrations)
├── Common/        # Infra compartilhada (Security, Logging, Messaging)
├── IoC/           # Injeção de dependência
└── Tests/         # Testes unitários e de integração

Padrões e Princípios-Chave

Clean Architecture: Domínio independente de frameworks, dependências sempre fluindo para dentro.

CQRS + MediatR: Separação entre comandos (escrita) e queries (leitura).

Repository Pattern: Acesso a dados abstraído em interfaces.

Specification Pattern: Regras de consulta reutilizáveis e composáveis.

Pipeline Behavior: Validação e logging centralizados.

Mensageria assíncrona: Fila de mensagens via sistema de arquivos.

🔧 Stack e Ferramentas

.NET 9, Entity Framework Core, SQL Server

MediatR, AutoMapper, FluentValidation

JWT + BCrypt para autenticação e segurança

Serilog para logging estruturado e health checks

xUnit, Moq, Bogus, FluentAssertions para testes

Swagger com autenticação integrada

🔑 Segurança e Observabilidade

JWT Bearer flexível (aceita token com ou sem prefixo Bearer)

Hash seguro de senhas com BCrypt

Middleware global de tratamento de exceções

Health checks para banco, mensageria e dependências

Logging otimizado com rotação diária e retenção de 7 dias

🚀 Fluxo de Dados
Request → Controller → MediatR → Handler → Repository → Banco
                ↓
Response ← DTO ← Mapper ← Result ← Entity ← ORM
                ↓
Message → MessageService → Queue (File System)

🧪 Testes

111 testes passando (unitários, de integração e de domínio)

Cobertura de casos de uso, controllers e serviços de infraestrutura

Ferramentas: xUnit, Moq, FluentAssertions e Bogus

💡 Benefícios Arquiteturais

Manutenibilidade: código desacoplado e organizado

Escalabilidade: pronto para crescer em volume e features

Flexibilidade: troca de implementações sem impacto no domínio

Segurança e Resiliência: autenticação robusta e health checks

Alta Testabilidade: camadas isoladas facilitam cobertura automatizada

📌 Conclusão

O CleanCode demonstra domínio em arquitetura moderna e padrões consagrados, aplicando:

Clean Architecture + CQRS + SOLID

Repository e Specification Pattern

Mensageria assíncrona e logging estruturado

Resultado: um sistema robusto, escalável e totalmente testado, pronto para produção e fácil de evoluir.
