// Константы используемые во всех микросервисах
// Содержит названия очередей, ключи кэша, настройки и т.д.
namespace CADStorage.Common.Constants;

/// <summary>
/// Константы для имен очередей RabbitMQ
/// </summary>
public static class QueueNames
{
    public const string UserRegistered = "user.registered";
    public const string ModelUploaded = "model.uploaded";
    public const string ModelPurchased = "model.purchased";
    public const string AdvertisementShown = "advertisement.shown";
    public const string NotificationSend = "notification.send";
    public const string SearchIndex = "search.index";
}

/// <summary>
/// Константы для ключей кэша Redis
/// </summary>
public static class CacheKeys
{
    public const string UserProfilePrefix = "user:profile:";
    public const string ModelDetailsPrefix = "model:details:";
    public const string CatalogPagePrefix = "catalog:page:";
    public const string AdvertisementPlacementPrefix = "ads:placement:";
    public const string SearchResultsPrefix = "search:results:";
    
    // Время жизни кэша в минутах
    public static readonly TimeSpan UserProfileExpiration = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan ModelDetailsExpiration = TimeSpan.FromMinutes(60);
    public static readonly TimeSpan CatalogPageExpiration = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan AdvertisementPlacementExpiration = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan SearchResultsExpiration = TimeSpan.FromMinutes(10);
}

/// <summary>
/// Константы для настроек рекламы
/// </summary>
public static class AdvertisementSettings
{
    // Плотность рекламы: каждое N-ое место в сетке
    public const int AdFrequency = 5;
    
    // Минимальное количество моделей перед первым рекламным блоком
    public const int MinModelsBeforeFirstAd = 3;
    
    // Максимальное количество рекламных блоков на странице
    public const int MaxAdsPerPage = 3;
}

/// <summary>
/// Константы для настроек файлов
/// </summary>
public static class FileSettings
{
    // Максимальный размер файла в байтах (100 МБ)
    public const long MaxFileSize = 100 * 1024 * 1024;
    
    // Разрешенные расширения файлов
    public static readonly string[] AllowedExtensions = { ".step", ".stp", ".igs", ".iges", ".xt", ".x_t", ".x_b", ".pdf" };
    
    // MIME типы для STEP файлов
    public const string StepMimeType = "application/step";
}

/// <summary>
/// Константы для настроек безопасности
/// </summary>
public static class SecuritySettings
{
    // Минимальная длина пароля
    public const int MinPasswordLength = 8;
    
    // Время жизни токена доступа в минутах
    public const int AccessTokenExpirationMinutes = 60;
    
    // Время жизни токена обновления в днях
    public const int RefreshTokenExpirationDays = 7;
}
