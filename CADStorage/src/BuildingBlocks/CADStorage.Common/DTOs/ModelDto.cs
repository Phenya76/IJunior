// DTO для передачи данных о 3D модели между микросервисами
namespace CADStorage.Common.DTOs;

/// <summary>
/// Данные о 3D модели для передачи между сервисами
/// </summary>
public record ModelDto
{
    /// <summary>
    /// Уникальный идентификатор модели
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Название модели
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Описание модели
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор автора (владельца) модели
    /// </summary>
    public Guid AuthorId { get; init; }

    /// <summary>
    /// Данные автора (краткие)
    /// </summary>
    public UserBriefDto? Author { get; init; }

    /// <summary>
    /// Категория модели
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Теги для поиска
    /// </summary>
    public string[] Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Цена модели (0 если бесплатная)
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Валюта цены
    /// </summary>
    public string Currency { get; init; } = "RUB";

    /// <summary>
    /// URL превью изображения модели
    /// </summary>
    public string? PreviewUrl { get; init; }

    /// <summary>
    /// URL файла модели (STEP и др.)
    /// </summary>
    public string? FileUrl { get; init; }

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// Формат файла (STEP, IGES, Parasolid и т.д.)
    /// </summary>
    public string FileFormat { get; init; } = string.Empty;

    /// <summary>
    /// Рейтинг модели (средняя оценка)
    /// </summary>
    public decimal Rating { get; init; }

    /// <summary>
    /// Количество отзывов
    /// </summary>
    public int ReviewsCount { get; init; }

    /// <summary>
    /// Количество скачиваний
    /// </summary>
    public int DownloadsCount { get; init; }

    /// <summary>
    /// Количество просмотров
    /// </summary>
    public int ViewsCount { get; init; }

    /// <summary>
    /// Статус модели (Draft, Pending, Published, Archived, Rejected)
    /// </summary>
    public string Status { get; init; } = "Draft";

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Является ли модель бесплатной
    /// </summary>
    public bool IsFree => Price == 0;
}

/// <summary>
/// Краткие данные модели для отображения в каталоге
/// </summary>
public record ModelBriefDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? PreviewUrl { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "RUB";
    public decimal Rating { get; init; }
    public int ReviewsCount { get; init; }
    public int DownloadsCount { get; init; }
    public UserBriefDto? Author { get; init; }
    public bool IsFree => Price == 0;
}
