// Метод расширения для регистрации общих сервисов в DI контейнере
namespace Microsoft.Extensions.DependencyInjection;

using CADStorage.Common.Interfaces;
using CADStorage.Common.Services;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using MassTransit;

/// <summary>
/// Методы расширения для регистрации сервисов из Common библиотеки
/// </summary>
public static class CommonServiceCollectionExtensions
{
    /// <summary>
    /// Зарегистрировать общие сервисы (кэширование, публикация событий)
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="redisConnectionString">Строка подключения к Redis</param>
    /// <returns>Коллекция сервисов для продолжения цепочки вызовов</returns>
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        string redisConnectionString)
    {
        // Регистрация сервиса кэширования Redis
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });

        services.AddSingleton<IDistributedCache>(sp =>
        {
            var connection = sp.GetRequiredService<IConnectionMultiplexer>();
            return new RedisCache(connection, new RedisCacheOptions());
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        // Публикатор событий регистрируется в каждом микросервисе через MassTransit
        // Здесь только интерфейс
        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        return services;
    }

    /// <summary>
    /// Зарегистрировать MassTransit с RabbitMQ
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="rabbitMqHost">Хост RabbitMQ</param>
    /// <param name="rabbitMqUsername">Имя пользователя RabbitMQ</param>
    /// <param name="rabbitMqPassword">Пароль RabbitMQ</param>
    /// <returns>Конфигурация MassTransit для продолжения настройки</returns>
    public static IServiceCollection AddCommonMassTransit(
        this IServiceCollection services,
        string rabbitMqHost,
        string rabbitMqUsername,
        string rabbitMqPassword)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqHost), h =>
                {
                    h.Username(rabbitMqUsername);
                    h.Password(rabbitMqPassword);
                });

                // Настройка политик повторных попыток
                x.UseRetry(retry =>
                {
                    retry.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1));
                });

                // Настройка логирования
                x.SetKestrelEndpointName("masstransit");
            });
        });

        return services;
    }
}
