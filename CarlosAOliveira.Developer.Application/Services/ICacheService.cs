namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Service for caching operations
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a value from cache
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Cached value or default</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Sets a value in cache
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Value to cache</param>
        /// <param name="expiration">Expiration time</param>
        /// <returns>Task</returns>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Removes a value from cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>Task</returns>
        Task RemoveAsync(string key);

        /// <summary>
        /// Removes multiple values from cache
        /// </summary>
        /// <param name="pattern">Pattern to match keys</param>
        /// <returns>Task</returns>
        Task RemoveByPatternAsync(string pattern);

        /// <summary>
        /// Checks if a key exists in cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(string key);
    }
}
