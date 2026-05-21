// Точка входа в микросервис рекламы
namespace CADStorage.Advertisement.Service;

using Microsoft.EntityFrameworkCore;
using CADStorage.Advertisement.Service.Data;
using CADStorage.Common.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// НАСТРОЙКА КОНФИДЕНЦИАЛЬНОСТИ
// ============================================

// Чтение настроек из appsettings.json или переменных окружения
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? "localhost:6379";
    
var rabbitMqHost = builder.Configuration.GetConnectionString("RabbitMQ") 
    ?? "rabbitmq://localhost";
    
var postgresConnectionString = builder.Configuration.GetConnectionString("PostgreSQL") 
    ?? "Host=localhost;Port=5432;Database=cadstorage_ads;Username=postgres;Password=postgres";

// ============================================
// РЕГИСТРАЦИЯ СЕРВИСОВ (Dependency Injection)
// ============================================

// Регистрация контекста базы данных PostgreSQL
builder.Services.AddDbContext<AdvertisementDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

// Регистрация общих сервисов (Redis кэш, MassTransit)
builder.Services.AddCommonServices(redisConnectionString);

// Регистрация MassTransit с RabbitMQ для работы с событиями
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqHost), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Настройка потребителя событий
        cfg.ReceiveEndpoint("advertisement-service", e =>
        {
            // Потребитель события загрузки модели
            e.ConfigureConsumer<ModelUploadedEventConsumer>(context);
        });

        // Политики повторных попыток при ошибках
        x.UseRetry(retry =>
        {
            retry.Exponential(
                retryLimit: 5,
                initialInterval: TimeSpan.FromSeconds(1),
                maxInterval: TimeSpan.FromMinutes(1),
                interval: TimeSpan.FromSeconds(1)
            );
        });
    });

    // Регистрация потребителей событий
    x.AddConsumer<ModelUploadedEventConsumer>();
});

// Добавление контроллеров API
builder.Services.AddControllers();

// Настройка Swagger для документации API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "CADStorage Advertisement Service",
        Version = "v1",
        Description = "Микросервис управления рекламой и контекстными рекламными вставками"
    });
});

// Настройка CORS для фронтенда
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Адрес фронтенда
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ============================================
// НАСТРОЙКА HTTP КОНВЕЙЕРА
// ============================================

var app = builder.Build();

// Настройка pipeline обработки HTTP запросов

// Swagger UI только в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Advertisement Service V1");
    });
}

// Использование CORS
app.UseCors("AllowFrontend");

// Аутентификация (будет добавлена позже)
// app.UseAuthentication();
// app.UseAuthorization();

// Маршрутизация запросов к контроллерам
app.MapControllers();

// ============================================
// ИНИЦИАЛИЗАЦИЯ БАЗЫ ДАННЫХ
// ============================================

// Автоматическое применение миграций при запуске
// В продакшене лучше применять миграции вручную через CLI
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AdvertisementDbContext>();
    
    try
    {
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✅ Миграции базы данных успешно применены");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка применения миграций: {ex.Message}");
    }
}

// ============================================
// ЗАПУСК ПРИЛОЖЕНИЯ
// ============================================

Console.WriteLine("🚀 CADStorage Advertisement Service запущен!");
Console.WriteLine($"📍 Swagger: http://localhost:5003/swagger");
Console.WriteLine($"🔗 API: http://localhost:5003/api/advertisement");

app.Run();
