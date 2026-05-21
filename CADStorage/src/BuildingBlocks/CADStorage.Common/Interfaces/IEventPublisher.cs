// Интерфейс для публикации событий через шину сообщений (RabbitMQ)
// Используется во всех микросервисах для отправки событий
namespace CADStorage.Common.Interfaces;

using MassTransit;

public interface IEventPublisher
{
    /// <summary>
    /// Опубликовать событие в шину сообщений
    /// Все подписчики получат это событие асинхронно
    /// </summary>
    /// <typeparam name="T">Тип события (должен быть record классом)</typeparam>
    /// <param name="eventData">Данные события</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Задача выполнения операции</returns>
    Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class;
}
