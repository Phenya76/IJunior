// DTO (Data Transfer Object) для передачи данных о пользователе между микросервисами
// Используется в API ответах и событиях
namespace CADStorage.Common.DTOs;

/// <summary>
/// Данные пользователя для передачи между сервисами
/// </summary>
public record UserDto
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Email адрес пользователя
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Имя пользователя (отображаемое имя)
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Аватар пользователя (URL изображения)
    /// </summary>
    public string? AvatarUrl { get; init; }

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime RegisteredAt { get; init; }

    /// <summary>
    /// Роль пользователя (User, Author, Admin)
    /// </summary>
    public string Role { get; init; } = "User";

    /// <summary>
    /// Рейтинг пользователя (средняя оценка работ)
    /// </summary>
    public decimal Rating { get; init; }

    /// <summary>
    /// Количество загруженных моделей
    /// </summary>
    public int ModelsCount { get; init; }
}

/// <summary>
/// Краткие данные пользователя для отображения в списках
/// </summary>
public record UserBriefDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public decimal Rating { get; init; }
}
