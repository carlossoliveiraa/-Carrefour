# 🏦 CleanCode - Sistema de Controle de Fluxo de Caixa

## 📋 Visão Geral

Este projeto implementa uma **Arquitetura Limpa (Clean Architecture)** seguindo os princípios SOLID e as melhores práticas de desenvolvimento de software. A aplicação é uma API RESTful construída em .NET 9.0 que gerencia um **sistema completo de controle de fluxo de caixa** para comerciantes, incluindo:

- 🔐 **Autenticação JWT** com Bearer token flexível
- 👥 **Gestão de usuários** com roles e status
- 💰 **Controle de transações** (débitos e créditos)
- 📊 **Consolidação de saldo diário** automática
- 📨 **Sistema de mensageria** com filas
- 📝 **Logging otimizado** e monitoramento
- 🧪 **Cobertura completa de testes** (111 testes)

## 🏗️ Arquitetura

### Estrutura de Camadas

O projeto está organizado em camadas bem definidas, seguindo os princípios da Clean Architecture:

```
CleanCode/
├── CleanCode.Api/           # 🌐 Camada de Apresentação (Web API + Controllers)
├── CleanCode.Application/   # 🚀 Camada de Aplicação (Use Cases + CQRS)
├── CleanCode.Domain/        # 🏛️ Camada de Domínio (Entidades + Regras de Negócio)
├── CleanCode.ORM/           # 🗄️ Camada de Infraestrutura (Acesso a Dados)
├── CleanCode.Common/        # 🔧 Infraestrutura Compartilhada (Security + Logging)
├── CleanCode.IoC/           # 🔌 Injeção de Dependência (DI Container)
└── CleanCode.tests/         # 🧪 Testes Unitários e de Integração
```

### 🎯 Princípios Arquiteturais

#### 1. **Separação de Responsabilidades**
- Cada camada tem uma responsabilidade específica e bem definida
- Dependências fluem sempre para dentro (em direção ao domínio)
- O domínio não depende de nenhuma camada externa

#### 2. **Inversão de Dependência**
- Interfaces definidas no domínio
- Implementações na camada de infraestrutura
- Uso de injeção de dependência para resolver dependências

#### 3. **CQRS (Command Query Responsibility Segregation)**
- Separação clara entre comandos (escrita) e consultas (leitura)
- Implementado com MediatR para desacoplamento

## 🔧 Tecnologias e Padrões

### Stack Tecnológico
- **.NET 9.0** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server** - Banco de dados relacional
- **MediatR** - Implementação do padrão Mediator/CQRS
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de dados
- **JWT Bearer** - Autenticação e autorização flexível
- **BCrypt** - Hash de senhas
- **Serilog** - Logging estruturado otimizado
- **Swagger** - Documentação da API com autenticação
- **xUnit + Moq + FluentAssertions** - Testes unitários
- **Bogus** - Geração de dados de teste

### Padrões Implementados

#### 1. **Repository Pattern**
```csharp
public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    Task<List<Transaction>> GetByDateAsync(DateTime date, CancellationToken cancellationToken);
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction> UpdateAsync(Transaction transaction, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
```

#### 2. **CQRS com MediatR**
```csharp
// Command para criar transação
public class CreateTransactionCommand : IRequest<CreateTransactionResult>
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Category { get; set; }
    public string? Notes { get; set; }
}

// Handler com mensageria
public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMessageService _messageService;
    
    public async Task<CreateTransactionResult> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
    {
        var transaction = _mapper.Map<Transaction>(command);
        var createdTransaction = await _transactionRepository.CreateAsync(transaction, cancellationToken);
        
        // Envia mensagem para fila
        await _messageService.SendMessageAsync(new TransactionCreatedMessage
        {
            TransactionId = createdTransaction.Id,
            Description = createdTransaction.Description,
            Amount = createdTransaction.Amount,
            Type = createdTransaction.Type.ToString(),
            TransactionDate = createdTransaction.TransactionDate,
            CreatedAt = DateTime.UtcNow
        }, "transaction_created", cancellationToken);
        
        return _mapper.Map<CreateTransactionResult>(createdTransaction);
    }
}
```

#### 3. **Specification Pattern**
```csharp
public class TransactionByDateSpecification : ISpecification<Transaction>
{
    private readonly DateTime _date;
    
    public TransactionByDateSpecification(DateTime date)
    {
        _date = date.Date;
    }
    
    public Expression<Func<Transaction, bool>> Criteria => 
        transaction => transaction.TransactionDate.Date == _date;
}

public class TransactionByTypeSpecification : ISpecification<Transaction>
{
    private readonly TransactionType _type;
    
    public TransactionByTypeSpecification(TransactionType type)
    {
        _type = type;
    }
    
    public Expression<Func<Transaction, bool>> Criteria => 
        transaction => transaction.Type == _type;
}
```

#### 4. **Pipeline Behavior (Cross-cutting Concerns)**
```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
            
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }
        
        return await next();
    }
}
```

#### 5. **Messaging Pattern**
```csharp
public interface IMessageService
{
    Task SendMessageAsync<T>(T message, string queueName, CancellationToken cancellationToken = default);
}

// Implementação com filas de arquivo
public class FileMessageService : IMessageService
{
    public async Task SendMessageAsync<T>(T message, string queueName, CancellationToken cancellationToken = default)
    {
        var queuePath = Path.Combine(_basePath, queueName);
        Directory.CreateDirectory(queuePath);
        
        var fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}_{Guid.NewGuid():N}.json";
        var filePath = Path.Combine(queuePath, fileName);
        
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}
```

## 📁 Estrutura Detalhada das Camadas

### 🎨 **CleanCode.Api** - Camada de Apresentação
- **Controllers**: Endpoints RESTful organizados por features
- **Middleware**: Tratamento de exceções e logging de requests
- **DTOs**: Objetos de transferência de dados para API
- **Mappings**: Configuração do AutoMapper
- **Swagger**: Documentação interativa com autenticação

**Características:**
- Controllers finos que apenas orquestram chamadas
- Validação de entrada com FluentValidation
- Respostas padronizadas com `ApiResponse`
- Middleware para tratamento global de exceções
- Autenticação JWT com Bearer token flexível
- Swagger com botão de autenticação e cadeado visual

### 🚀 **CleanCode.Application** - Camada de Aplicação
- **Commands/Queries**: Definição de casos de uso
- **Handlers**: Implementação da lógica de negócio
- **Validators**: Regras de validação específicas
- **Profiles**: Mapeamentos AutoMapper
- **Messaging**: Integração com sistema de mensageria

**Características:**
- Implementação do padrão CQRS
- Handlers focados em uma única responsabilidade
- Validação de regras de negócio
- Mapeamento entre camadas
- Integração com sistema de mensageria
- Consolidação automática de saldo diário

### 🏛️ **CleanCode.Domain** - Camada de Domínio
- **Entities**: Entidades de negócio com comportamento
- **Enums**: Tipos enumerados do domínio
- **Specifications**: Regras de consulta reutilizáveis
- **Validation**: Validações de domínio
- **Interfaces**: Contratos para repositórios

**Entidades Principais:**
- **User**: Usuário do sistema com roles e status
- **Transaction**: Transação de fluxo de caixa (débito/crédito)
- **DailyBalance**: Saldo consolidado diário
- **LogEntry**: Entrada de log do sistema

**Características:**
- Entidades ricas com comportamento
- Regras de negócio encapsuladas
- Independente de frameworks externos
- Domain Events para comunicação entre agregados
- Validações de domínio com FluentValidation

### 🗄️ **CleanCode.ORM** - Camada de Infraestrutura (Dados)
- **DbContext**: Configuração do Entity Framework
- **Repositories**: Implementação dos repositórios
- **Migrations**: Controle de versão do banco de dados
- **Configurations**: Configurações de mapeamento das entidades

**Repositórios Implementados:**
- **UserRepository**: Operações com usuários
- **TransactionRepository**: Operações com transações
- **DailyBalanceRepository**: Operações com saldo diário

**Características:**
- Implementação concreta dos repositórios
- Configuração de mapeamento ORM
- Migrations para evolução do banco
- Suporte a consultas complexas com Specifications

### 🔧 **CleanCode.Common** - Infraestrutura Compartilhada
- **Security**: Autenticação JWT e hash de senhas
- **Validation**: Comportamentos de validação
- **Logging**: Configuração de logging otimizado
- **HealthChecks**: Monitoramento da aplicação
- **Messaging**: Sistema de mensageria com filas

### 🔌 **CleanCode.IoC** - Injeção de Dependência
- **ModuleInitializers**: Configuração modular de dependências
- **DependencyResolver**: Resolução centralizada de dependências

### 🧪 **CleanCode.tests** - Testes Unitários e de Integração
- **UnitTests**: Testes unitários para handlers e controllers
- **Mocks**: Uso de Moq para simulação de dependências
- **FluentAssertions**: Assertions expressivas e legíveis
- **Bogus**: Geração de dados de teste realistas

**Cobertura de Testes:**
- ✅ **111 testes passando**
- ✅ **Handlers**: Todos os handlers testados
- ✅ **Controllers**: Todos os controllers testados
- ✅ **Services**: Serviços de infraestrutura testados
- ✅ **HealthChecks**: Monitoramento testado

## 🔐 Segurança

### Autenticação JWT
- Tokens JWT com expiração configurável (padrão: 24 horas)
- Claims para identificação do usuário
- Middleware de autenticação configurado
- **Bearer token flexível**: Aceita com ou sem "Bearer"
- Swagger com botão de autenticação e cadeado visual

### Hash de Senhas
- BCrypt para hash seguro de senhas
- Verificação de senhas com salt automático

### Validação
- Validação em múltiplas camadas
- FluentValidation para validação declarativa
- Middleware global para tratamento de erros de validação

## 📊 Monitoramento e Logging

### Logging Estruturado
- Serilog para logging estruturado
- **Logging otimizado**: Apenas erros após inicialização
- Logs em arquivo com rotação diária
- Middleware para logging de requests
- Retenção de 7 dias de logs

### Health Checks
- Endpoints de health check
- Monitoramento de dependências
- Verificação de conectividade com banco
- Monitoramento do sistema de mensageria

### Sistema de Mensageria
- **Filas organizadas**: Por tipo de operação
- **Mensagens automáticas**: Enviadas em todos os POSTs
- **Estrutura de filas**:
  - `transaction_created/` - Transações criadas
  - `transaction_updated/` - Transações atualizadas
  - `transaction_deleted/` - Transações deletadas
  - `daily_balance_consolidated/` - Saldos consolidados
  - `user_accessed/` - Usuários acessados

## 🚀 Executando o Projeto

### Pré-requisitos
- .NET 9.0 SDK
- SQL Server (local ou Docker)
- Visual Studio 2022 ou VS Code

### Configuração
1. Clone o repositório
2. Configure a connection string no `appsettings.json`
3. Execute as migrations: `dotnet ef database update --project CleanCode.ORM`
4. Execute o projeto: `dotnet run --project CleanCode.Api`
5. Acesse o Swagger: `https://localhost:7000/swagger`

### Docker
```bash
docker-compose up -d
```

## 📝 Endpoints da API

### 🔓 **Endpoints Sem Autenticação**
- `POST /api/auth` - Autenticar usuário
- `POST /api/users` - Criar usuário
- `GET /api/users/{id}` - Buscar usuário por ID

### 🔒 **Endpoints Com Autenticação JWT**

#### **Transações**
- `POST /api/transactions` - Criar transação
- `GET /api/transactions/{id}` - Buscar transação por ID
- `GET /api/transactions` - Listar transações com filtros

#### **Saldo Diário**
- `GET /api/dailybalance` - Buscar saldo diário por data
- `POST /api/dailybalance/consolidate` - Consolidar saldo diário

### 🔑 **Como Usar a Autenticação**
1. **Fazer Login**: `POST /api/auth` com email e senha
2. **Copiar Token**: Da resposta da autenticação
3. **Usar Token**: No header `Authorization: Bearer TOKEN` ou `Authorization: TOKEN`
4. **Swagger**: Clique em "Authorize" e cole o token

## 🧪 Testes

### **Cobertura Completa de Testes**
- ✅ **111 testes passando**
- ✅ **Testes unitários** para handlers
- ✅ **Testes de integração** para controllers
- ✅ **Testes de domínio** para entidades
- ✅ **Testes de serviços** de infraestrutura
- ✅ **Testes de health checks**

### **Ferramentas de Teste**
- **xUnit**: Framework de testes
- **Moq**: Mocking de dependências
- **FluentAssertions**: Assertions expressivas
- **Bogus**: Geração de dados de teste

### **Executar Testes**
```bash
dotnet test CleanCode.tests/CleanCode.tests.csproj
```

## 📈 Benefícios da Arquitetura

1. **Manutenibilidade**: Código organizado e fácil de manter
2. **Testabilidade**: Camadas desacopladas facilitam testes
3. **Escalabilidade**: Estrutura preparada para crescimento
4. **Flexibilidade**: Fácil troca de implementações
5. **Separação de Responsabilidades**: Cada camada tem seu propósito
6. **Reutilização**: Componentes podem ser reutilizados
7. **Padrões Consistentes**: Uso consistente de padrões estabelecidos
8. **Segurança**: Autenticação JWT robusta e flexível
9. **Monitoramento**: Logging otimizado e health checks
10. **Mensageria**: Sistema de filas para comunicação assíncrona

## 🔄 Fluxo de Dados

```
Request → Controller → MediatR → Handler → Repository → Database
                ↓
Response ← DTO ← Mapper ← Result ← Entity ← ORM
                ↓
Message → MessageService → Queue → File System
```

## 🎯 **Demonstração de Conhecimento em Arquitetura**

### **🏗️ Padrões Arquiteturais Implementados**

#### **1. Clean Architecture (Arquitetura Limpa)**
- **Separação clara de responsabilidades** entre camadas
- **Dependências fluem para dentro** (em direção ao domínio)
- **Domínio independente** de frameworks externos
- **Inversão de dependência** com interfaces

#### **2. CQRS (Command Query Responsibility Segregation)**
- **Separação entre comandos e consultas**
- **Handlers especializados** para cada operação
- **MediatR** para desacoplamento
- **Validação automática** com pipeline behaviors

#### **3. Repository Pattern**
- **Abstração do acesso a dados**
- **Interfaces no domínio**, implementações na infraestrutura
- **Testabilidade** com mocks
- **Flexibilidade** para troca de implementações

#### **4. Specification Pattern**
- **Regras de consulta reutilizáveis**
- **Composição de critérios** complexos
- **Separação de lógica de consulta** do repositório

#### **5. Pipeline Pattern**
- **Cross-cutting concerns** (validação, logging)
- **Comportamento consistente** em toda aplicação
- **Extensibilidade** para novos comportamentos

### **🔧 Padrões de Design Implementados**

#### **1. Mediator Pattern**
- **Desacoplamento** entre controllers e handlers
- **Comunicação centralizada** via MediatR
- **Facilita testes** e manutenção

#### **2. Strategy Pattern**
- **Diferentes estratégias** de validação
- **Diferentes estratégias** de mapeamento
- **Extensibilidade** para novas estratégias

#### **3. Factory Pattern**
- **Criação de objetos** complexos
- **Configuração centralizada** de dependências
- **Injeção de dependência** automática

#### **4. Observer Pattern**
- **Sistema de mensageria** para eventos
- **Desacoplamento** entre operações
- **Comunicação assíncrona**

### **🛡️ Princípios SOLID Aplicados**

#### **S - Single Responsibility Principle**
- **Cada classe tem uma responsabilidade** específica
- **Handlers** focados em uma operação
- **Controllers** apenas orquestram chamadas

#### **O - Open/Closed Principle**
- **Extensível** para novas funcionalidades
- **Fechado** para modificações desnecessárias
- **Pipeline behaviors** para extensão

#### **L - Liskov Substitution Principle**
- **Interfaces** bem definidas
- **Implementações** intercambiáveis
- **Testes** com mocks funcionam perfeitamente

#### **I - Interface Segregation Principle**
- **Interfaces específicas** para cada responsabilidade
- **Repositórios** com métodos específicos
- **Serviços** com responsabilidades claras

#### **D - Dependency Inversion Principle**
- **Dependências** de abstrações, não implementações
- **Injeção de dependência** em toda aplicação
- **Domínio** não depende de infraestrutura

### **📊 Métricas de Qualidade**

- ✅ **111 testes passando** (100% de sucesso)
- ✅ **0 erros de compilação**
- ✅ **Cobertura completa** de funcionalidades
- ✅ **Logging otimizado** (apenas erros)
- ✅ **Autenticação robusta** com JWT flexível
- ✅ **Sistema de mensageria** funcional
- ✅ **Health checks** implementados
- ✅ **Swagger** com autenticação visual

### **🚀 Funcionalidades Avançadas**

#### **1. Sistema de Fluxo de Caixa Completo**
- **Transações** de débito e crédito
- **Consolidação automática** de saldo diário
- **Filtros avançados** por data, tipo, categoria
- **Validações de negócio** robustas

#### **2. Autenticação JWT Flexível**
- **Bearer token automático** (com ou sem "Bearer")
- **Swagger integrado** com autenticação visual
- **Claims** para identificação de usuário
- **Expiração configurável**

#### **3. Sistema de Mensageria**
- **Filas organizadas** por tipo de operação
- **Mensagens automáticas** em todos os POSTs
- **Comunicação assíncrona** entre componentes
- **Monitoramento** de filas

 