// DTO для рекламного объявления
namespace CADStorage.Common.DTOs;

/// <summary>
/// Данные рекламного объявления
/// </summary>
public record AdvertisementDto
{
    /// <summary>
    /// Уникальный идентификатор объявления
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Название рекламы (для внутреннего использования)
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Текст рекламного объявления
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// URL изображения рекламы
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// URL перехода при клике (на сайт рекламодателя или модель)
    /// </summary>
    public string ClickUrl { get; init; } = string.Empty;

    /// <summary>
    /// Тип рекламы (ModelPromotion, Banner, Contextual)
    /// </summary>
    public string Type { get; init; } = "Contextual";

    /// <summary>
    /// Идентификатор модели (если реклама продвигает конкретную модель)
    /// </summary>
    public Guid? ModelId { get; init; }

    /// <summary>
    /// Идентификатор рекламодателя
    /// </summary>
    public Guid AdvertiserId { get; init; }

    /// <summary>
    /// Дата начала показа
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Дата окончания показа
    /// </summary>
    public DateTime EndDate { get; init; }

    /// <summary>
    /// Статус рекламы (Draft, Active, Paused, Expired, Rejected)
    /// </summary>
    public string Status { get; init; } = "Draft";

    /// <summary>
    /// Количество показов
    /// </summary>
    public int ImpressionsCount { get; init; }

    /// <summary>
    /// Количество кликов
    /// </summary>
    public int ClicksCount { get; init; }

    /// <summary>
    /// CTR (Click-Through Rate) в процентах
    /// </summary>
    public double CTR => ImpressionsCount > 0 ? (double)ClicksCount / ImpressionsCount * 100 : 0;

    /// <summary>
    /// Цена за клик
    /// </summary>
    public decimal CostPerClick { get; init; }

    /// <summary>
    /// Бюджет кампании
    /// </summary>
    public decimal Budget { get; init; }

    /// <summary>
    /// Потрачено средств
    /// </summary>
    public decimal SpentAmount { get; init; }
}

/// <summary>
/// Краткие данные рекламы для вставки в каталог
/// </summary>
public record AdvertisementBriefDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public string ClickUrl { get; init; } = string.Empty;
    public string Type { get; init; } = "Contextual";
}
