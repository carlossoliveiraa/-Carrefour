using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// In-memory cache service implementation
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Gets a value from cache
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Cached value or default</returns>
        public Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    if (value is string jsonString)
                    {
                        var result = JsonSerializer.Deserialize<T>(jsonString);
                        return Task.FromResult(result);
                    }
                    
                    if (value is T directValue)
                    {
                        return Task.FromResult(directValue);
                    }
                }

                return Task.FromResult(default(T?));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from cache for key: {Key}", key);
                return Task.FromResult(default(T?));
            }
        }

        /// <summary>
        /// Sets a value in cache
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Value to cache</param>
        /// <param name="expiration">Expiration time</param>
        /// <returns>Task</returns>
        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = expiration ?? TimeSpan.FromMinutes(15),
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
                };

                // Serialize complex objects to JSON
                if (typeof(T).IsClass && typeof(T) != typeof(string))
                {
                    var jsonString = JsonSerializer.Serialize(value);
                    _cache.Set(key, jsonString, options);
                }
                else
                {
                    _cache.Set(key, value, options);
                }

                _logger.LogDebug("Value cached for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Removes a value from cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>Task</returns>
        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _logger.LogDebug("Value removed from cache for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing value from cache for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Removes multiple values from cache
        /// Note: MemoryCache doesn't support pattern-based removal, so this is a no-op
        /// </summary>
        /// <param name="pattern">Pattern to match keys</param>
        /// <returns>Task</returns>
        public Task RemoveByPatternAsync(string pattern)
        {
            _logger.LogWarning("Pattern-based cache removal not supported in MemoryCache. Pattern: {Pattern}", pattern);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Checks if a key exists in cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>True if exists, false otherwise</returns>
        public Task<bool> ExistsAsync(string key)
        {
            try
            {
                var exists = _cache.TryGetValue(key, out _);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if key exists in cache: {Key}", key);
                return Task.FromResult(false);
            }
        }
    }
}
