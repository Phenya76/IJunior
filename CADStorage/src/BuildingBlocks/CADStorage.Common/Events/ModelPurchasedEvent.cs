// Событие: Покупка модели совершена
// Используется для обновления статистики, начисления средств автору и отправки уведомлений
namespace CADStorage.Common.Events;

public record ModelPurchasedEvent
{
    /// <summary>
    /// Уникальный идентификатор заказа
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Идентификатор покупателя
    /// </summary>
    public Guid BuyerId { get; init; }

    /// <summary>
    /// Идентификатор автора модели (продавца)
    /// </summary>
    public Guid AuthorId { get; init; }

    /// <summary>
    /// Идентификатор купленной модели
    /// </summary>
    public Guid ModelId { get; init; }

    /// <summary>
    /// Название модели
    /// </summary>
    public string ModelName { get; init; } = string.Empty;

    /// <summary>
    /// Сумма покупки
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Валюта платежа
    /// </summary>
    public string Currency { get; init; } = "RUB";

    /// <summary>
    /// Дата и время покупки
    /// </summary>
    public DateTime PurchasedAt { get; init; } = DateTime.UtcNow;
}
