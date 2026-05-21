// Базовый класс для всех сущностей в системе
// Содержит общие поля: идентификатор, дата создания, дата обновления
namespace CADStorage.Common.Entities;

public abstract class BaseEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата и время последнего обновления записи
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
