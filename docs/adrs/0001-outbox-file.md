# ADR 0001: Decisão da Fila em Arquivo (Outbox Pattern Simplificado)

## Status

Aceito

## Contexto

Precisamos implementar um sistema de cashflow com desacoplamento entre a API (produtor de transações) e o Worker (consumidor que atualiza o saldo consolidado). O requisito é que o serviço de lançamentos continue funcionando mesmo se o consolidado estiver fora.

## Decisão

Implementamos um sistema de fila usando arquivo NDJSON como "fila" append-only, em vez de usar sistemas de mensageria como RabbitMQ, Apache Kafka ou Azure Service Bus.

### Implementação

- **Arquivo de Fila**: `./runtime/queue.ndjson` (append-only)
- **Formato**: Uma linha JSON por evento
- **Checkpoint**: `./runtime/checkpoint.txt` com offset do último processado
- **Idempotência**: `./runtime/processed.ids` com IDs já processados
- **Polling**: Worker lê a cada 200ms

## Alternativas Consideradas

1. **RabbitMQ**: Complexidade de infraestrutura, requer servidor separado
2. **Apache Kafka**: Overkill para o escopo, complexidade de setup
3. **Azure Service Bus**: Dependência de cloud, custos
4. **Database Outbox**: Poderia causar locks na tabela principal
5. **In-Memory Queue**: Não persiste entre reinicializações

## Consequências

### Positivas

- ✅ **Simplicidade**: Fácil de entender e debugar
- ✅ **Visibilidade**: Eventos são visíveis em texto plano
- ✅ **Sem Dependências**: Não requer infraestrutura externa
- ✅ **Desacoplamento**: API e Worker são independentes
- ✅ **Eventual Consistency**: Garantida pela natureza append-only
- ✅ **Crash Recovery**: Worker retoma do último checkpoint

### Negativas

- ❌ **Performance**: Limitada para alta concorrência
- ❌ **Escalabilidade**: Não suporta múltiplos workers
- ❌ **Durabilidade**: Arquivo pode ser corrompido
- ❌ **Limpeza**: Eventos antigos não são removidos automaticamente
- ❌ **Concorrência**: Múltiplos writers podem causar problemas

## Trade-offs

| Aspecto | Fila em Arquivo | RabbitMQ | Database Outbox |
|---------|----------------|----------|-----------------|
| Simplicidade | ⭐⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐ |
| Performance | ⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ |
| Durabilidade | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ |
| Setup | ⭐⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐⭐ |
| Debugging | ⭐⭐⭐⭐⭐ | ⭐⭐ | ⭐⭐⭐ |

## Conclusão

Para o escopo atual (50 req/s, ambiente local, simplicidade), a fila em arquivo é a melhor opção. Em produção com maior escala, seria necessário migrar para uma solução mais robusta como RabbitMQ ou Apache Kafka.
