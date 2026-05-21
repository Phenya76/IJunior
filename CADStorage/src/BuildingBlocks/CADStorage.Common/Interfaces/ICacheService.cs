// Интерфейс для кэширования данных
// Используется для распределенного кэша Redis между микросервисами
namespace CADStorage.Common.Interfaces;

public interface ICacheService
{
    /// <summary>
    /// Получить данные из кэша по ключу
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <param name="key">Ключ кэша</param>
    /// <returns>Данные из кэша или null если не найдено</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Сохранить данные в кэш с временем жизни
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <param name="key">Ключ кэша</param>
    /// <param name="value">Значение для сохранения</param>
    /// <param name="expiration">Время жизни кэша</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача выполнения операции</returns>
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить данные из кэша по ключу
    /// </summary>
    /// <param name="key">Ключ кэша</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача выполнения операции</returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Очистить все кэшированные данные (использовать с осторожностью!)
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача выполнения операции</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
