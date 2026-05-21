// Событие: Пользователь зарегистрировался в системе
// Используется для отправки приветственного письма и уведомления других сервисов
namespace CADStorage.Common.Events;

public record UserRegisteredEvent
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Email адрес пользователя
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Дата и время регистрации
    /// </summary>
    public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;
}
