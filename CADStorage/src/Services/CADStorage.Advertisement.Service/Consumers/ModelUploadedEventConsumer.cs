// Потребитель событий: Модель загружена
// Слушает события от Model.Service и создает контекстную рекламу
namespace CADStorage.Advertisement.Service.Consumers;

using MassTransit;
using CADStorage.Common.Events;
using CADStorage.Common.Constants;
using Microsoft.EntityFrameworkCore;
using CADStorage.Advertisement.Service.Data;
using CADStorage.Advertisement.Service.Entities;

/// <summary>
/// Потребитель события загрузки модели
/// Анализирует модель и подбирает релевантную рекламу
/// </summary>
public class ModelUploadedEventConsumer : IConsumer<ModelUploadedEvent>
{
    private readonly AdvertisementDbContext _dbContext;
    private readonly ILogger<ModelUploadedEventConsumer> _logger;

    public ModelUploadedEventConsumer(
        AdvertisementDbContext dbContext,
        ILogger<ModelUploadedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Обработка события загрузки модели
    /// </summary>
    public async Task Consume(ConsumeContext<ModelUploadedEvent> context)
    {
        var eventData = context.Message;
        
        _logger.LogInformation("Получено событие загрузки модели: {ModelId}, Категория: {Category}", 
            eventData.ModelId, eventData.Category);

        try
        {
            // Найти активные рекламы с таргетингом на эту категорию или теги
            var relevantAds = await _dbContext.Advertisements
                .Where(a => a.Status == "Active"
                           && a.StartDate <= DateTime.UtcNow
                           && a.EndDate >= DateTime.UtcNow
                           && (a.TargetKeywords.Contains(eventData.Category)
                               || eventData.Tags.Any(tag => a.TargetKeywords.Contains(tag))))
                .OrderByDescending(a => a.Priority)
                .Take(3)
                .ToListAsync(context.CancellationToken);

            if (relevantAds.Any())
            {
                _logger.LogInformation("Найдено {Count} релевантных реклам для модели {ModelId}", 
                    relevantAds.Count, eventData.ModelId);
                
                // Здесь можно отправить событие о показе рекламы
                // или сохранить в кэш для быстрого доступа
            }
            else
            {
                _logger.LogInformation("Не найдено релевантных реклам для модели {ModelId}", 
                    eventData.ModelId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки события загрузки модели {ModelId}", 
                eventData.ModelId);
            
            // Не выбрасываем исключение чтобы не блокировать обработку других событий
        }
    }
}
