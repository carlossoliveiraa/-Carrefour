using CarlosAOliveira.Developer.Application.Services;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Domain.Repositories;
using CarlosAOliveira.Developer.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Text.Json;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Create host builder
var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<DefaultContext>(options =>
            options.UseSqlite("Data Source=./runtime/cashflow.db"));
        
        services.AddScoped<IDailyBalanceRepository, CarlosAOliveira.Developer.ORM.Repositories.DailyBalanceRepository>();
        services.AddScoped<IEventQueue, FileEventQueue>();
        services.AddScoped<ICheckpointService, FileCheckpointService>();
    })
    .Build();

// Handle command line arguments
if (args.Length > 0 && args[0] == "--selftest")
{
    await RunSelfTest();
    return;
}

// Run the worker
await RunWorker();

async Task RunSelfTest()
{
    Log.Information("Running self-test...");
    
    try
    {
        using var scope = host.Services.CreateScope();
        var checkpointService = scope.ServiceProvider.GetRequiredService<ICheckpointService>();
        var eventQueue = scope.ServiceProvider.GetRequiredService<IEventQueue>();
        
        // Test checkpoint service
        var checkpointTest = await checkpointService.SelfTestAsync();
        Log.Information("Checkpoint service test: {Result}", checkpointTest ? "PASSED" : "FAILED");
        
        // Test event queue
        var fileSize = await eventQueue.GetFileSizeAsync();
        Log.Information("Event queue file size: {Size} bytes", fileSize);
        
        // Test database connection
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        await context.Database.EnsureCreatedAsync();
        Log.Information("Database connection test: PASSED");
        
        Log.Information("Self-test completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Self-test failed");
        Environment.Exit(1);
    }
}

async Task RunWorker()
{
    Log.Information("Starting Cashflow Worker...");
    
    using var scope = host.Services.CreateScope();
    var eventQueue = scope.ServiceProvider.GetRequiredService<IEventQueue>();
    var checkpointService = scope.ServiceProvider.GetRequiredService<ICheckpointService>();
    var dailyBalanceRepository = scope.ServiceProvider.GetRequiredService<IDailyBalanceRepository>();
    var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
    
    // Ensure database is created
    await context.Database.EnsureCreatedAsync();
    
    var lastOffset = await checkpointService.GetLastOffsetAsync();
    Log.Information("Starting from offset: {Offset}", lastOffset);
    
    var cancellationTokenSource = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cancellationTokenSource.Cancel();
        Log.Information("Shutdown requested...");
    };
    
    try
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var events = await eventQueue.ReadEventsAsync(lastOffset, cancellationTokenSource.Token);
                
                foreach (var queueEvent in events)
                {
                    // Skip if already processed
                    if (await checkpointService.IsEventProcessedAsync(queueEvent.EventId))
                    {
                        Log.Debug("Skipping already processed event: {EventId}", queueEvent.EventId);
                        lastOffset = queueEvent.Offset;
                        continue;
                    }
                    
                    // Process the event
                    await ProcessEvent(queueEvent, dailyBalanceRepository);
                    
                    // Mark as processed and update checkpoint
                    await checkpointService.MarkEventAsProcessedAsync(queueEvent.EventId);
                    await checkpointService.UpdateOffsetAsync(queueEvent.Offset);
                    lastOffset = queueEvent.Offset;
                    
                    Log.Debug("Processed event: {EventId} of type {Type}", queueEvent.EventId, queueEvent.Type);
                }
                
                // Wait before next poll
                await Task.Delay(200, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing events");
                await Task.Delay(1000, cancellationTokenSource.Token); // Wait longer on error
            }
        }
    }
    finally
    {
        Log.Information("Worker stopped");
    }
}

async Task ProcessEvent(QueueEvent queueEvent, IDailyBalanceRepository dailyBalanceRepository)
{
    try
    {
        switch (queueEvent.Type)
        {
            case "TransactionCreatedEvent":
                await ProcessTransactionCreated(queueEvent.Payload, dailyBalanceRepository);
                break;
            case "TransactionDeletedEvent":
                await ProcessTransactionDeleted(queueEvent.Payload, dailyBalanceRepository);
                break;
            default:
                Log.Warning("Unknown event type: {Type}", queueEvent.Type);
                break;
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error processing event {EventId} of type {Type}", queueEvent.EventId, queueEvent.Type);
        throw;
    }
}

async Task ProcessTransactionCreated(object payload, IDailyBalanceRepository dailyBalanceRepository)
{
    var json = JsonSerializer.Serialize(payload);
    var eventData = JsonSerializer.Deserialize<TransactionCreatedEventData>(json);
    
    if (eventData == null) return;
    
    var amount = eventData.TransactionType == "Credit" ? eventData.Amount : -eventData.Amount;
    
    var existingBalance = await dailyBalanceRepository.GetByDateAsync(eventData.Date);
    if (existingBalance != null)
    {
        existingBalance.AddAmount(amount);
        await dailyBalanceRepository.UpdateAsync(existingBalance);
    }
    else
    {
        var newBalance = new CarlosAOliveira.Developer.Domain.Entities.DailyBalance(eventData.Date, amount);
        await dailyBalanceRepository.AddAsync(newBalance);
    }
    
    await dailyBalanceRepository.SaveChangesAsync();
}

async Task ProcessTransactionDeleted(object payload, IDailyBalanceRepository dailyBalanceRepository)
{
    var json = JsonSerializer.Serialize(payload);
    var eventData = JsonSerializer.Deserialize<TransactionDeletedEventData>(json);
    
    if (eventData == null) return;
    
    var amount = eventData.TransactionType == "Credit" ? -eventData.Amount : eventData.Amount; // Reverse the effect
    
    var existingBalance = await dailyBalanceRepository.GetByDateAsync(eventData.Date);
    if (existingBalance != null)
    {
        existingBalance.AddAmount(amount);
        await dailyBalanceRepository.UpdateAsync(existingBalance);
        await dailyBalanceRepository.SaveChangesAsync();
    }
}

// DTOs for JSON deserialization
public record TransactionCreatedEventData(DateOnly Date, decimal Amount, string TransactionType);
public record TransactionDeletedEventData(DateOnly Date, decimal Amount, string TransactionType);
