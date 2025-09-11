# Cashflow System

Sistema de lançamentos financeiros diários com consolidado de saldo por dia, implementado em .NET 9 com arquitetura modular.

## 🏗️ Arquitetura

- **Monólito Modular**: Mesma solução com camadas bem definidas
- **Worker Separado**: Console application para processamento de eventos
- **Fila em Arquivo**: Sistema de mensagens usando arquivo NDJSON
- **Banco SQLite**: Por padrão, com suporte opcional ao PostgreSQL
- **Eventual Consistency**: Desacoplamento entre API e Worker

## 📁 Estrutura do Projeto

```
CarlosAOliveira.Developer/
├── CarlosAOliveira.Developer.Api/          # API REST
├── CarlosAOliveira.Developer.Worker/       # Worker Console
├── CarlosAOliveira.Developer.Application/  # Casos de Uso
├── CarlosAOliveira.Developer.Domain/       # Entidades e Regras
├── CarlosAOliveira.Developer.ORM/          # Acesso a Dados
├── CarlosAOliveira.Developer.Common/       # Utilitários
└── CarlosAOliveira.Developer.Tests/        # Testes
```

## 🚀 Como Executar

### Pré-requisitos

- .NET 9.0 SDK
- Visual Studio 2022 ou VS Code

### 1. Executar a API

```bash
cd CarlosAOliveira.Developer.Api
dotnet run
```

A API estará disponível em: `https://localhost:7001`

### 2. Executar o Worker

Em um terminal separado:

```bash
cd CarlosAOliveira.Developer.Worker
dotnet run
```

### 3. Testar o Sistema

```bash
# Self-test do Worker
cd CarlosAOliveira.Developer.Worker
dotnet run --selftest
```

## 📊 Endpoints da API

### Autenticação

Primeiro, faça login para obter o token JWT:

```bash
curl -X POST "https://localhost:7001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "Admin123!"
  }'
```

### Transações

#### Criar Transação

```bash
curl -X POST "https://localhost:7001/api/cashflow/transactions" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Idempotency-Key: unique-key-123" \
  -d '{
    "date": "2024-01-15",
    "amount": 1000.50,
    "type": "Credit",
    "category": "Sales",
    "description": "Venda de produto"
  }'
```

#### Buscar Transação

```bash
curl -X GET "https://localhost:7001/api/cashflow/transactions/{id}" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Saldo Consolidado

#### Buscar Saldo Diário

```bash
curl -X GET "https://localhost:7001/api/cashflow/consolidated/daily?date=2024-01-15" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Health Check

```bash
curl -X GET "https://localhost:7001/health"
```

## 🔄 Sistema de Fila em Arquivo

### Como Funciona

1. **Produtor (API)**: Após criar uma transação, apenda o evento no arquivo `./runtime/queue.ndjson`
2. **Consumidor (Worker)**: Lê o arquivo a cada 200ms, processa novos eventos e atualiza o saldo diário
3. **Checkpoint**: Mantém `./runtime/checkpoint.txt` com o offset do último evento processado
4. **Idempotência**: Mantém `./runtime/processed.ids` com IDs de eventos já processados

### Formato do Evento

```json
{
  "eventId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "TransactionCreated",
  "payload": {
    "transactionId": "550e8400-e29b-41d4-a716-446655440001",
    "date": "2024-01-15",
    "amount": 1000.50,
    "transactionType": "Credit",
    "category": "Sales",
    "description": "Venda de produto",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "ts": "2024-01-15T10:30:00Z"
}
```

## 🗄️ Banco de Dados

### SQLite (Padrão)

O banco é criado automaticamente em `./runtime/cashflow.db`

### PostgreSQL (Opcional)

Para usar PostgreSQL, configure a connection string no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=cashflow;User Id=postgres;Password=password;"
  }
}
```

## 🧪 Testes

### Executar Testes Unitários

```bash
dotnet test
```

### Teste de Carga (50 req/s)

```bash
# Script de teste de carga será implementado
dotnet run --project LoadTest
```

## 📈 Performance

- **Meta**: 50 requisições/segundo no endpoint de consolidado
- **Tolerância**: Até 5% de perda sob pico
- **Índices**: Otimizados para consultas por data

## 🔒 Segurança

- **JWT Bearer**: Autenticação obrigatória
- **HTTPS**: Habilitado por padrão
- **Headers de Segurança**: X-Content-Type-Options, X-Frame-Options, HSTS
- **Idempotência**: Header Idempotency-Key obrigatório

## 📝 Logs

- **API**: `./logs/carrefour-api-YYYY-MM-DD.log`
- **Worker**: `./logs/worker-YYYY-MM-DD.txt`
- **Formato**: Estruturado com Serilog
- **Correlation ID**: Rastreamento de requisições

## 🛠️ Desenvolvimento

### Estrutura de Camadas

- **Domain**: Entidades, eventos e regras de negócio
- **Application**: Casos de uso, comandos e queries
- **ORM**: Acesso a dados e repositórios
- **API**: Controllers e DTOs
- **Worker**: Processamento de eventos

### Padrões Utilizados

- **CQRS**: Separação de comandos e queries
- **MediatR**: Mediator pattern para desacoplamento
- **Repository**: Padrão de acesso a dados
- **Domain Events**: Eventos de domínio para comunicação

## 🚨 Limitações

- **Fila em Arquivo**: Não é adequada para alta concorrência
- **SQLite**: Limitado a um processo por vez
- **Worker Único**: Não há distribuição de carga
- **Memória**: Eventos processados ficam em arquivo (não há limpeza automática)

## 📚 Documentação Adicional

- [ADR 0001: Decisão da Fila em Arquivo](./docs/adrs/0001-outbox-file.md)
- [Exemplos de Requisições](./http/)

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
