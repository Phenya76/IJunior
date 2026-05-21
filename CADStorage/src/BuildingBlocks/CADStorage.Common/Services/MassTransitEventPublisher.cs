// Реализация публикатора событий через MassTransit (RabbitMQ)
namespace CADStorage.Common.Services;

using MassTransit;
using CADStorage.Common.Interfaces;

/// <summary>
/// Сервис публикации событий в шину сообщений RabbitMQ
/// Используется для асинхронной коммуникации между микросервисами
/// </summary>
public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventPublisher> _logger;

    /// <summary>
    /// Конструктор публикатора событий
    /// </summary>
    /// <param name="publishEndpoint">Интерфейс публикации MassTransit</param>
    /// <param name="logger">Логгер для записи событий</param>
    public MassTransitEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <summary>
    /// Опубликовать событие в шину сообщений
    /// Все подписчики получат это событие асинхронно
    /// </summary>
    public async Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var eventType = typeof(T).Name;
            
            _logger.LogInformation("Публикация события: {EventType}", eventType);
            
            await _publishEndpoint.Publish(eventData, cancellationToken);
            
            _logger.LogInformation("Событие {EventType} успешно опубликовано", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка публикации события типа {EventType}", typeof(T).Name);
            
            // В продакшене здесь можно добавить логику повторных попыток
            // или сохранение события в базу для последующей отправки
            throw;
        }
    }
}
