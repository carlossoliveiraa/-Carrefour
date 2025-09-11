# 🚀 Guia de Deploy - Sistema Cashflow

## 📋 Visão Geral

Este guia detalha como fazer o deploy do Sistema Cashflow em diferentes ambientes, desde desenvolvimento até produção.

## 🛠️ Pré-requisitos

### Desenvolvimento
- **.NET 9.0 SDK**
- **Visual Studio 2022** ou **VS Code**
- **SQL Server** ou **SQLite**
- **Git**

### Produção
- **.NET 9.0 Runtime**
- **SQL Server** ou **PostgreSQL**
- **IIS** ou **Nginx** (opcional)
- **Redis** (opcional, para cache distribuído)

## 🏗️ Estrutura do Projeto

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

## 🔧 Configuração

### 1. **Configuração de Desenvolvimento**

#### appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=cashflow_dev;User Id=sa;Password=123;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "development-secret-key-very-long-and-secure",
    "Issuer": "CashflowAPI",
    "Audience": "CashflowClient",
    "ExpirationMinutes": 60
  },
  "RateLimit": {
    "MaxRequests": 1000,
    "WindowMinutes": 1
  },
  "Cache": {
    "DefaultExpirationMinutes": 15
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

### 2. **Configuração de Produção**

#### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=cashflow_prod;User Id=cashflow_user;Password=secure_password;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "production-secret-key-very-long-and-secure",
    "Issuer": "CashflowAPI",
    "Audience": "CashflowClient",
    "ExpirationMinutes": 30
  },
  "RateLimit": {
    "MaxRequests": 100,
    "WindowMinutes": 1
  },
  "Cache": {
    "DefaultExpirationMinutes": 30
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

## 🚀 Deploy Local

### 1. **Clone do Repositório**
```bash
git clone https://github.com/seu-usuario/cashflow-system.git
cd cashflow-system
```

### 2. **Restaurar Dependências**
```bash
dotnet restore
```

### 3. **Configurar Banco de Dados**
```bash
# Criar migração (se necessário)
dotnet ef migrations add InitialCreate --project CarlosAOliveira.Developer.ORM --startup-project CarlosAOliveira.Developer.Api

# Aplicar migrações
dotnet ef database update --project CarlosAOliveira.Developer.ORM --startup-project CarlosAOliveira.Developer.Api
```

### 4. **Executar Testes**
```bash
dotnet test
```

### 5. **Executar API**
```bash
dotnet run --project CarlosAOliveira.Developer.Api
```

### 6. **Executar Worker** (Terminal separado)
```bash
dotnet run --project CarlosAOliveira.Developer.Worker
```

## 🐳 Deploy com Docker

### 1. **Dockerfile para API**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CarlosAOliveira.Developer.Api/CarlosAOliveira.Developer.Api.csproj", "CarlosAOliveira.Developer.Api/"]
COPY ["CarlosAOliveira.Developer.Application/CarlosAOliveira.Developer.Application.csproj", "CarlosAOliveira.Developer.Application/"]
COPY ["CarlosAOliveira.Developer.Domain/CarlosAOliveira.Developer.Domain.csproj", "CarlosAOliveira.Developer.Domain/"]
COPY ["CarlosAOliveira.Developer.ORM/CarlosAOliveira.Developer.ORM.csproj", "CarlosAOliveira.Developer.ORM/"]
COPY ["CarlosAOliveira.Developer.Common/CarlosAOliveira.Developer.Common.csproj", "CarlosAOliveira.Developer.Common/"]
RUN dotnet restore "CarlosAOliveira.Developer.Api/CarlosAOliveira.Developer.Api.csproj"
COPY . .
WORKDIR "/src/CarlosAOliveira.Developer.Api"
RUN dotnet build "CarlosAOliveira.Developer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarlosAOliveira.Developer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarlosAOliveira.Developer.Api.dll"]
```

### 2. **Dockerfile para Worker**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CarlosAOliveira.Developer.Worker/CarlosAOliveira.Developer.Worker.csproj", "CarlosAOliveira.Developer.Worker/"]
COPY ["CarlosAOliveira.Developer.Application/CarlosAOliveira.Developer.Application.csproj", "CarlosAOliveira.Developer.Application/"]
COPY ["CarlosAOliveira.Developer.Domain/CarlosAOliveira.Developer.Domain.csproj", "CarlosAOliveira.Developer.Domain/"]
COPY ["CarlosAOliveira.Developer.ORM/CarlosAOliveira.Developer.ORM.csproj", "CarlosAOliveira.Developer.ORM/"]
COPY ["CarlosAOliveira.Developer.Common/CarlosAOliveira.Developer.Common.csproj", "CarlosAOliveira.Developer.Common/"]
RUN dotnet restore "CarlosAOliveira.Developer.Worker/CarlosAOliveira.Developer.Worker.csproj"
COPY . .
WORKDIR "/src/CarlosAOliveira.Developer.Worker"
RUN dotnet build "CarlosAOliveira.Developer.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarlosAOliveira.Developer.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarlosAOliveira.Developer.Worker.dll"]
```

### 3. **Docker Compose**
```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: CarlosAOliveira.Developer.Api/Dockerfile
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=cashflow;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
    depends_on:
      - db
    volumes:
      - ./logs:/app/logs

  worker:
    build:
      context: .
      dockerfile: CarlosAOliveira.Developer.Worker/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=cashflow;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
    depends_on:
      - db
    volumes:
      - ./runtime:/app/runtime
      - ./logs:/app/logs

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

volumes:
  sqlserver_data:
  redis_data:
```

### 4. **Executar com Docker Compose**
```bash
# Build e execução
docker-compose up --build

# Execução em background
docker-compose up -d

# Parar serviços
docker-compose down

# Parar e remover volumes
docker-compose down -v
```

## ☁️ Deploy em Produção

### 1. **Azure App Service**

#### Configuração
```bash
# Criar resource group
az group create --name cashflow-rg --location eastus

# Criar app service plan
az appservice plan create --name cashflow-plan --resource-group cashflow-rg --sku B1

# Criar web app
az webapp create --name cashflow-api --resource-group cashflow-rg --plan cashflow-plan --runtime "DOTNET|9.0"

# Configurar connection string
az webapp config connection-string set --name cashflow-api --resource-group cashflow-rg --connection-string-type SQLServer --settings DefaultConnection="Server=tcp:your-server.database.windows.net,1433;Initial Catalog=cashflow;Persist Security Info=False;User ID=your-user;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

#### Deploy
```bash
# Deploy via Azure CLI
az webapp deployment source config --name cashflow-api --resource-group cashflow-rg --repo-url https://github.com/seu-usuario/cashflow-system.git --branch main --manual-integration

# Deploy via ZIP
dotnet publish -c Release
Compress-Archive -Path .\bin\Release\net9.0\publish\* -DestinationPath deploy.zip
az webapp deployment source config-zip --name cashflow-api --resource-group cashflow-rg --src deploy.zip
```

### 2. **AWS Elastic Beanstalk**

#### Configuração
```bash
# Instalar EB CLI
pip install awsebcli

# Inicializar aplicação
eb init

# Criar ambiente
eb create production

# Deploy
eb deploy
```

### 3. **Google Cloud Run**

#### Configuração
```bash
# Configurar projeto
gcloud config set project your-project-id

# Build e push da imagem
gcloud builds submit --tag gcr.io/your-project-id/cashflow-api

# Deploy
gcloud run deploy cashflow-api --image gcr.io/your-project-id/cashflow-api --platform managed --region us-central1 --allow-unauthenticated
```

## 🔒 Configuração de Segurança

### 1. **HTTPS**
```csharp
// Program.cs
app.UseHttpsRedirection();
app.UseHsts();
```

### 2. **CORS**
```csharp
// Program.cs
app.UseCors("AllowSpecificOrigins");

// Configuração
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### 3. **Headers de Segurança**
```csharp
// Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

## 📊 Monitoramento

### 1. **Application Insights (Azure)**
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### 2. **Health Checks**
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddRedis(redisConnectionString);

app.MapHealthChecks("/health");
```

### 3. **Logs**
```csharp
// Program.cs
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/cashflow-api-.log", rollingInterval: RollingInterval.Day)
        .WriteTo.ApplicationInsights(context.Configuration.GetConnectionString("ApplicationInsights"), TelemetryConverter.Traces);
});
```

## 🧪 Testes de Deploy

### 1. **Testes de Saúde**
```bash
# Health check
curl https://your-api-url/health

# Teste de autenticação
curl -X POST https://your-api-url/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123!"}'
```

### 2. **Testes de Performance**
```bash
# Teste de carga com Apache Bench
ab -n 1000 -c 10 https://your-api-url/health

# Teste de carga com Artillery
artillery quick --count 100 --num 10 https://your-api-url/health
```

### 3. **Testes de Funcionalidade**
```bash
# Criar transação
curl -X POST https://your-api-url/api/cashflow/transactions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Idempotency-Key: test-123" \
  -d '{"date":"2024-01-15","amount":100,"type":"Credit","category":"Test","description":"Test transaction"}'
```

## 🔧 Troubleshooting

### 1. **Problemas Comuns**

#### Erro de Conexão com Banco
```bash
# Verificar connection string
# Verificar se o banco está acessível
# Verificar credenciais
```

#### Erro de Migração
```bash
# Verificar se o banco existe
# Verificar permissões
# Executar migrações manualmente
```

#### Erro de Autenticação
```bash
# Verificar JWT settings
# Verificar se o token está válido
# Verificar se o usuário existe
```

### 2. **Logs**
```bash
# Ver logs da aplicação
tail -f logs/cashflow-api-$(date +%Y-%m-%d).log

# Ver logs do Docker
docker logs cashflow-api

# Ver logs do Azure
az webapp log tail --name cashflow-api --resource-group cashflow-rg
```

### 3. **Métricas**
```bash
# Ver métricas de performance
# Verificar uso de memória
# Verificar tempo de resposta
# Verificar taxa de erro
```

## 📈 Otimizações de Produção

### 1. **Performance**
- **Cache**: Implementar Redis para cache distribuído
- **CDN**: Usar CDN para assets estáticos
- **Load Balancer**: Implementar load balancer para alta disponibilidade
- **Database**: Otimizar queries e índices

### 2. **Escalabilidade**
- **Horizontal Scaling**: Múltiplas instâncias da API
- **Database Sharding**: Particionamento de dados
- **Microservices**: Quebrar em microserviços menores
- **Event Sourcing**: Para auditoria e replay

### 3. **Monitoramento**
- **APM**: Application Performance Monitoring
- **Logs**: Centralização de logs (ELK Stack)
- **Métricas**: Prometheus + Grafana
- **Alertas**: Alertas automáticos para problemas

## 🎯 Checklist de Deploy

### Pré-Deploy
- [ ] Testes unitários passando
- [ ] Testes de integração passando
- [ ] Configuração de ambiente correta
- [ ] Secrets e connection strings configurados
- [ ] Backup do banco de dados

### Deploy
- [ ] Deploy da aplicação
- [ ] Aplicação de migrações
- [ ] Verificação de health checks
- [ ] Testes de funcionalidade
- [ ] Verificação de logs

### Pós-Deploy
- [ ] Monitoramento ativo
- [ ] Alertas configurados
- [ ] Backup automático
- [ ] Documentação atualizada
- [ ] Equipe notificada

---

**Guia de Deploy - Sistema Cashflow** 🚀
