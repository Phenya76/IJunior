// Реализация сервиса кэширования на основе Redis
namespace CADStorage.Common.Services;

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using CADStorage.Common.Interfaces;

/// <summary>
/// Сервис распределенного кэширования с использованием Redis
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    /// <summary>
    /// Конструктор сервиса кэширования
    /// </summary>
    /// <param name="cache">Интерфейс распределенного кэша</param>
    /// <param name="logger">Логгер для записи событий</param>
    public RedisCacheService(
        IDistributedCache cache,
        ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Получить данные из кэша по ключу
    /// </summary>
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key);
            
            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogDebug("Кэш-промах для ключа: {Key}", key);
                return default;
            }

            _logger.LogDebug("Кэш-попадание для ключа: {Key}", key);
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения данных из кэша по ключу: {Key}", key);
            return default;
        }
    }

    /// <summary>
    /// Сохранить данные в кэш с временем жизни
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                SlidingExpiration = TimeSpan.FromMinutes(5) // Обновлять при частом доступе
            };

            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
            
            _logger.LogDebug("Данные сохранены в кэш по ключу: {Key}, время жизни: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка сохранения данных в кэш по ключу: {Key}", key);
        }
    }

    /// <summary>
    /// Удалить данные из кэша по ключу
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Данные удалены из кэша по ключу: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка удаления данных из кэша по ключу: {Key}", key);
        }
    }

    /// <summary>
    /// Очистить все кэшированные данные
    /// Внимание! Эта операция может быть дорогостоящей в продакшене
    /// </summary>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Redis не поддерживает полную очистку через IDistributedCache
            // В реальной реализации нужно использовать отдельный механизм
            _logger.LogWarning("Попытка полной очистки кэша. Требуется ручное вмешательство.");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка очистки кэша");
        }
    }
}
