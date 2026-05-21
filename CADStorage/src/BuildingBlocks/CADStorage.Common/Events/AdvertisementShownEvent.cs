// Событие: Рекламное объявление было показано пользователю
// Используется для статистики и биллинга рекламодателя
namespace CADStorage.Common.Events;

public record AdvertisementShownEvent
{
    /// <summary>
    /// Уникальный идентификатор показа рекламы
    /// </summary>
    public Guid ImpressionId { get; init; }

    /// <summary>
    /// Идентификатор рекламного объявления
    /// </summary>
    public Guid AdId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, которому показана реклама (может быть null если не авторизован)
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Идентификатор модели, рядом с которой показана реклама
    /// </summary>
    public Guid? ModelId { get; init; }

    /// <summary>
    /// Позиция рекламы в сетке (индекс)
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    /// Дата и время показа
    /// </summary>
    public DateTime ShownAt { get; init; } = DateTime.UtcNow;
}
