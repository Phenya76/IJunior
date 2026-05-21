// Контроллер для управления рекламой и получения рекламных вставок
namespace CADStorage.Advertisement.Service.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CADStorage.Advertisement.Service.Data;
using CADStorage.Advertisement.Service.Entities;
using CADStorage.Common.DTOs;
using CADStorage.Common.Interfaces;
using CADStorage.Common.Events;
using CADStorage.Common.Constants;

/// <summary>
/// Контроллер для работы с рекламой
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AdvertisementController : ControllerBase
{
    private readonly AdvertisementDbContext _dbContext;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<AdvertisementController> _logger;

    public AdvertisementController(
        AdvertisementDbContext dbContext,
        IEventPublisher eventPublisher,
        ILogger<AdvertisementController> logger)
    {
        _dbContext = dbContext;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Получить рекламу для вставки в каталог моделей
    /// Возвращает список рекламы с указанием позиций для вставки
    /// </summary>
    /// <param name="modelsCount">Количество моделей в выдаче</param>
    /// <param name="category">Категория для таргетинга (опционально)</param>
    /// <returns>Список рекламных вставок с позициями</returns>
    [HttpGet("catalog-placements")]
    public async Task<ActionResult<List<AdPlacementResult>>> GetCatalogPlacements(
        [FromQuery] int modelsCount,
        [FromQuery] string? category = null)
    {
        try
        {
            // Найти активные рекламы
            var query = _dbContext.Advertisements
                .Where(a => a.Status == "Active"
                           && a.StartDate <= DateTime.UtcNow
                           && a.EndDate >= DateTime.UtcNow);

            // Если указана категория, фильтруем по ключевым словам
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.TargetKeywords.Contains(category));
            }

            var activeAds = await query
                .OrderByDescending(a => a.Priority)
                .Take(AdvertisementSettings.MaxAdsPerPage)
                .ToListAsync();

            if (!activeAds.Any())
            {
                return Ok(new List<AdPlacementResult>());
            }

            // Рассчитать позиции для вставки рекламы
            var placements = CalculateAdPlacements(activeAds, modelsCount);

            // Отправить события о показах (асинхронно, не дожидаясь ответа)
            foreach (var placement in placements)
            {
                var impressionEvent = new AdvertisementShownEvent
                {
                    ImpressionId = Guid.NewGuid(),
                    AdId = placement.Advertisement.Id,
                    UserId = null, // Можно получить из токена аутентификации
                    ModelId = null,
                    Position = placement.Position,
                    ShownAt = DateTime.UtcNow
                };

                // Публикуем событие в шину (не блокируя ответ)
                _ = _eventPublisher.PublishAsync(impressionEvent);
            }

            _logger.LogInformation("Возвращено {Count} рекламных размещений для {ModelsCount} моделей", 
                placements.Count, modelsCount);

            return Ok(placements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения рекламных размещений");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Расчет позиций для вставки рекламы в сетку моделей
    /// </summary>
    private List<AdPlacementResult> CalculateAdPlacements(
        List<Advertisement> ads, 
        int modelsCount)
    {
        var placements = new List<AdPlacementResult>();
        
        // Позиции для вставки: каждые N моделей, начиная с MinModelsBeforeFirstAd
        var positionsToFill = new List<int>();
        
        for (int i = AdvertisementSettings.MinModelsBeforeFirstAd; 
             i < modelsCount && positionsToFill.Count < AdvertisementSettings.MaxAdsPerPage; 
             i += AdvertisementSettings.AdFrequency)
        {
            positionsToFill.Add(i);
        }

        // Распределить рекламы по позициям
        for (int i = 0; i < Math.Min(ads.Count, positionsToFill.Count); i++)
        {
            placements.Add(new AdPlacementResult
            {
                Position = positionsToFill[i],
                Advertisement = new AdvertisementBriefDto
                {
                    Id = ads[i].Id,
                    Text = ads[i].Text,
                    ImageUrl = ads[i].ImageUrl,
                    ClickUrl = ads[i].ClickUrl,
                    Type = ads[i].Type
                }
            });
        }

        return placements;
    }

    /// <summary>
    /// Записать клик по рекламе
    /// </summary>
    [HttpPost("{adId:guid}/click")]
    public async Task<ActionResult> RecordClick(Guid adId)
    {
        try
        {
            var ad = await _dbContext.Advertisements.FindAsync(adId);
            
            if (ad == null)
            {
                return NotFound("Реклама не найдена");
            }

            if (ad.Status != "Active")
            {
                return BadRequest("Реклама не активна");
            }

            // Увеличить счетчик кликов
            ad.ClicksCount++;
            
            // Добавить сумму к потраченным средствам
            ad.SpentAmount += ad.CostPerClick;

            // Проверить не превышен ли бюджет
            if (ad.SpentAmount >= ad.Budget)
            {
                ad.Status = "Paused";
                _logger.LogInformation("Реклама {AdId} приостановлена из-за превышения бюджета", adId);
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Зарегистрирован клик по рекламе {AdId}", adId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка записи клика по рекламе {AdId}", adId);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Создать новое рекламное объявление
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AdvertisementDto>> CreateAdvertisement(
        [FromBody] CreateAdvertisementRequest request)
    {
        try
        {
            var advertisement = new Advertisement
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Text = request.Text,
                ImageUrl = request.ImageUrl,
                ClickUrl = request.ClickUrl,
                Type = request.Type,
                ModelId = request.ModelId,
                AdvertiserId = request.AdvertiserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = "Draft",
                CostPerClick = request.CostPerClick,
                Budget = request.Budget,
                Priority = request.Priority ?? 1,
                PlacementPositions = request.PlacementPositions ?? Array.Empty<string>(),
                TargetKeywords = request.TargetKeywords ?? Array.Empty<string>()
            };

            _dbContext.Advertisements.Add(advertisement);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Создано рекламное объявление {AdId}", advertisement.Id);

            return CreatedAtAction(nameof(GetAdvertisementById), new { id = advertisement.Id }, 
                MapToDto(advertisement));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка создания рекламного объявления");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить рекламу по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdvertisementDto>> GetAdvertisementById(Guid id)
    {
        var ad = await _dbContext.Advertisements.FindAsync(id);
        
        if (ad == null)
        {
            return NotFound("Реклама не найдена");
        }

        return Ok(MapToDto(ad));
    }

    /// <summary>
    /// Преобразование сущности в DTO
    /// </summary>
    private static AdvertisementDto MapToDto(Advertisement ad)
    {
        return new AdvertisementDto
        {
            Id = ad.Id,
            Title = ad.Title,
            Text = ad.Text,
            ImageUrl = ad.ImageUrl,
            ClickUrl = ad.ClickUrl,
            Type = ad.Type,
            ModelId = ad.ModelId,
            AdvertiserId = ad.AdvertiserId,
            StartDate = ad.StartDate,
            EndDate = ad.EndDate,
            Status = ad.Status,
            ImpressionsCount = ad.ImpressionsCount,
            ClicksCount = ad.ClicksCount,
            CostPerClick = ad.CostPerClick,
            Budget = ad.Budget,
            SpentAmount = ad.SpentAmount
        };
    }
}

/// <summary>
/// Результат расчета позиции для рекламной вставки
/// </summary>
public record AdPlacementResult
{
    /// <summary>
    /// Позиция в сетке (индекс)
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    /// Данные рекламного объявления
    /// </summary>
    public AdvertisementBriefDto Advertisement { get; init; } = null!;
}

/// <summary>
/// Запрос на создание рекламы
/// </summary>
public record CreateAdvertisementRequest
{
    public string Title { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public string ClickUrl { get; init; } = string.Empty;
    public string Type { get; init; } = "Contextual";
    public Guid? ModelId { get; init; }
    public Guid AdvertiserId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal CostPerClick { get; init; }
    public decimal Budget { get; init; }
    public int? Priority { get; init; }
    public string[]? PlacementPositions { get; init; }
    public string[]? TargetKeywords { get; init; }
}
