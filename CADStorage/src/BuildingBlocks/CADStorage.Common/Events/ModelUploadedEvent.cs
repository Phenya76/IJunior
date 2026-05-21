// Событие: Модель была загружена пользователем
// Используется для индексации модели в поисковом сервисе и уведомления модераторов
namespace CADStorage.Common.Events;

public record ModelUploadedEvent
{
    /// <summary>
    /// Уникальный идентификатор загруженной модели
    /// </summary>
    public Guid ModelId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, загрузившего модель
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Название модели
    /// </summary>
    public string ModelName { get; init; } = string.Empty;

    /// <summary>
    /// Описание модели
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Категория модели (например: "Детали", "Узлы", "Инструменты")
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Теги для поиска (массив строк)
    /// </summary>
    public string[] Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Цена модели (0 если бесплатная)
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Дата и время загрузки
    /// </summary>
    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;
}
