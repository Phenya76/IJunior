// Сущность рекламного объявления в базе данных
namespace CADStorage.Advertisement.Service.Entities;

using CADStorage.Common.Entities;

/// <summary>
/// Сущность рекламного объявления
/// Хранится в базе данных PostgreSQL
/// </summary>
public class Advertisement : BaseEntity
{
    /// <summary>
    /// Название рекламы (для внутреннего использования)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Текст рекламного объявления
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// URL изображения рекламы
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// URL перехода при клике
    /// </summary>
    public string ClickUrl { get; set; } = string.Empty;

    /// <summary>
    /// Тип рекламы (ModelPromotion, Banner, Contextual)
    /// </summary>
    public string Type { get; set; } = "Contextual";

    /// <summary>
    /// Идентификатор модели (если реклама продвигает конкретную модель)
    /// </summary>
    public Guid? ModelId { get; set; }

    /// <summary>
    /// Идентификатор рекламодателя
    /// </summary>
    public Guid AdvertiserId { get; set; }

    /// <summary>
    /// Дата начала показа
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Дата окончания показа
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Статус рекламы (Draft, Active, Paused, Expired, Rejected)
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// Количество показов
    /// </summary>
    public int ImpressionsCount { get; set; }

    /// <summary>
    /// Количество кликов
    /// </summary>
    public int ClicksCount { get; set; }

    /// <summary>
    /// Цена за клик
    /// </summary>
    public decimal CostPerClick { get; set; }

    /// <summary>
    /// Бюджет кампании
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Потрачено средств
    /// </summary>
    public decimal SpentAmount { get; set; }

    /// <summary>
    /// Приоритет показа (чем выше, тем чаще показывается)
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Позиции для показа (например: "catalog_grid", "model_page", "search_results")
    /// </summary>
    public string[] PlacementPositions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Ключевые слова для таргетинга (совпадение с тегами моделей)
    /// </summary>
    public string[] TargetKeywords { get; set; } = Array.Empty<string>();
}
