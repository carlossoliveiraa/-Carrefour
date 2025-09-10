# 🚀 Instruções para Executar a API Carrefour

## ✅ Pré-requisitos

1. **SQL Server** rodando no servidor `NOTBOOK`
   - Usuário: `sa`
   - Senha: `123`
   - Database: `carrefour` (será criada automaticamente)

2. **.NET 9.0 SDK** instalado

## 🏃‍♂️ Como Executar

### 1. Restaurar Dependências
```bash
dotnet restore
```

### 2. Criar e Atualizar o Banco de Dados
```bash
# Criar migração (se necessário)
dotnet ef migrations add InitialCreate --project CarlosAOliveira.Developer.ORM --startup-project CarlosAOliveira.Developer.Api

# Atualizar banco de dados
dotnet ef database update --project CarlosAOliveira.Developer.ORM --startup-project CarlosAOliveira.Developer.Api
```

### 3. Executar a API
```bash
dotnet run --project CarlosAOliveira.Developer.Api
```

## 🌐 Acessar a API

Após executar, a API estará disponível em:

- **Swagger UI**: `https://localhost:5001` ou `http://localhost:5000`
- **Health Check**: `https://localhost:5001/health`

## 🔐 Autenticação

### 1. Fazer Login
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "admin@carrefour.com",
  "password": "123456"
}
```

### 2. Usar o Token
Copie o `accessToken` da resposta e use no header:
```
Authorization: Bearer {seu_token_aqui}
```

## 📚 Endpoints Principais

### Autenticação
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh` - Renovar token
- `GET /api/auth/me` - Usuário atual

### Merchants
- `GET /api/merchants` - Listar merchants
- `POST /api/merchants` - Criar merchant
- `GET /api/merchants/{id}` - Obter merchant
- `PUT /api/merchants/{id}` - Atualizar merchant
- `DELETE /api/merchants/{id}` - Deletar merchant

### Transactions
- `GET /api/transactions/{id}` - Obter transação
- `POST /api/transactions` - Criar transação
- `GET /api/transactions/merchant/{merchantId}` - Transações por merchant

### Daily Summaries
- `GET /api/dailysummaries/merchant/{merchantId}/date/{date}` - Resumo diário
- `GET /api/dailysummaries/merchant/{merchantId}/period` - Resumo por período

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes específicos
dotnet test CarlosAOliveira.Developer.Tests
```

## 📊 Logs

Os logs são salvos em:
- **Console**: Durante execução
- **Arquivo**: `logs/carrefour-api-{data}.log`

## 🛠️ Troubleshooting

### Erro de Conexão com Banco
- Verifique se o SQL Server está rodando
- Confirme as credenciais no `appsettings.json`
- Teste a conexão: `Server=NOTBOOK;Database=carrefour;User Id=sa;Password=123;TrustServerCertificate=true;`

### Porta em Uso
- Altere as portas no `launchSettings.json`
- Ou mate o processo: `netstat -ano | findstr :5001`

### Swagger não Abre
- Acesse diretamente: `https://localhost:5001`
- Verifique se está em modo Development
- Confirme que `UseSwagger()` está configurado

## 🎯 Próximos Passos

1. Criar usuários de teste no banco
2. Testar todos os endpoints via Swagger
3. Configurar ambiente de produção
4. Implementar monitoramento

---

**Desenvolvido por Carlos Oliveira** 🚀
