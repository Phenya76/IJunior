// Контекст базы данных для сервиса рекламы
namespace CADStorage.Advertisement.Service.Data;

using Microsoft.EntityFrameworkCore;
using CADStorage.Advertisement.Service.Entities;

/// <summary>
/// Контекст базы данных для хранения рекламных объявлений
/// </summary>
public class AdvertisementDbContext : DbContext
{
    /// <summary>
    /// Конструктор контекста
    /// </summary>
    public AdvertisementDbContext(DbContextOptions<AdvertisementDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Таблица рекламных объявлений
    /// </summary>
    public DbSet<Advertisement> Advertisements { get; set; } = null!;

    /// <summary>
    /// Настройка модели базы данных
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка сущности Advertisement
        modelBuilder.Entity<Advertisement>(entity =>
        {
            // Имя таблицы в базе данных
            entity.ToTable("advertisements");

            // Первичный ключ
            entity.HasKey(e => e.Id);

            // Индексы для ускорения поиска
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
            entity.HasIndex(e => e.AdvertiserId);
            entity.HasIndex(e => e.ModelId);

            // Ограничения на длину строк
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Text)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(e => e.ClickUrl)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsRequired();

            // Проверка даты окончания (должна быть больше даты начала)
            entity.HasCheckConstraint(
                "CK_Advertisements_EndDate_GreaterThan_StartDate",
                "\"EndDate\" > \"StartDate\"");

            // Проверка бюджета (не может быть отрицательным)
            entity.HasCheckConstraint(
                "CK_Advertisements_Budget_NonNegative",
                "\"Budget\" >= 0");

            // Проверка цены за клик (не может быть отрицательной)
            entity.HasCheckConstraint(
                "CK_Advertisements_CostPerClick_NonNegative",
                "\"CostPerClick\" >= 0");
        });
    }

    /// <summary>
    /// Переопределение SaveChanges для автоматического обновления UpdatedAt
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Переопределение SaveChangesAsync для автоматического обновления UpdatedAt
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Обновление временных меток
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Advertisement>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
