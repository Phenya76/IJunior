# 📘 CADStorage.Advertisement.Service

Микросервис управления рекламой для маркетплейса 3D моделей CADStorage.

## 🎯 Назначение

- Управление рекламными объявлениями
- Контекстная реклама в каталоге моделей
- Таргетинг рекламы по категориям и тегам
- Учет показов и кликов
- Автоматическое списание бюджета

## 🏗️ Архитектура

```
CADStorage.Advertisement.Service/
├── Controllers/           # API контроллеры
│   └── AdvertisementController.cs
├── Entities/             # Сущности базы данных
│   └── Advertisement.cs
├── Data/                 # Контекст БД и миграции
│   └── AdvertisementDbContext.cs
├── Consumers/            # Потребители событий RabbitMQ
│   └── ModelUploadedEventConsumer.cs
├── DTOs/                 # Объекты передачи данных
├── Migrations/           # Миграции EF Core
├── Program.cs            # Точка входа
├── appsettings.json      # Конфигурация
└── CADStorage.Advertisement.Service.csproj
```

## 🔌 Интеграция с другими сервисами

### Получает события от:
- **Model.Service** → `ModelUploadedEvent` (для анализа и подбора релевантной рекламы)

### Отправляет события:
- **AdvertisementShownEvent** → для статистики и биллинга

### Зависимости:
- **PostgreSQL** - хранение данных о рекламе
- **Redis** - кэширование размещений
- **RabbitMQ** - шина событий

## 🚀 Запуск

### Требования:
- .NET 8 SDK
- PostgreSQL 15+
- Redis 7+
- RabbitMQ 3.12+

### Переменные окружения:

```bash
# База данных
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DB=cadstorage_ads
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

# Redis
REDIS_CONNECTION=localhost:6379

# RabbitMQ
RABBITMQ_HOST=rabbitmq://localhost
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
```

### Запуск через CLI:

```bash
cd CADStorage.Advertisement.Service
dotnet restore
dotnet run --urls=http://localhost:5003
```

## 📡 API Endpoints

### Получить размещения для каталога
```http
GET /api/advertisement/catalog-placements?modelsCount=20&category=Детали
```

**Ответ:**
```json
[
  {
    "position": 3,
    "advertisement": {
      "id": "guid",
      "text": "Рекламный текст",
      "imageUrl": "https://...",
      "clickUrl": "https://...",
      "type": "Contextual"
    }
  }
]
```

### Записать клик по рекламе
```http
POST /api/advertisement/{adId}/click
```

### Создать рекламу
```http
POST /api/advertisement
Content-Type: application/json

{
  "title": "Продвижение модели",
  "text": "Качественная 3D модель",
  "clickUrl": "https://...",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-12-31T23:59:59Z",
  "costPerClick": 10.50,
  "budget": 1000,
  "targetKeywords": ["Детали", "Инструменты"]
}
```

## 🎨 Логика работы контекстной рекламы

### Алгоритм вставки в каталог:

1. Фронтенд запрашивает `/catalog-placements` с количеством моделей
2. Сервис находит активные рекламы с подходящими ключевыми словами
3. Рассчитывает позиции: каждые 5 моделей, начиная с 3-й
4. Возвращает список размещений с позициями
5. Фронтенд вставляет рекламные карточки в сетку товаров
6. При показе отправляется событие `AdvertisementShownEvent`
7. При клике вызывается `/click` endpoint для списания средств

### Параметры таргетинга:

- **Категория модели** - совпадение с категорией
- **Теги модели** - совпадение с тегами
- **Приоритет** - чем выше, тем чаще показывается
- **Бюджет** - автоматическая пауза при превышении

## 📊 Мониторинг

### Логи:
- Создание/редактирование рекламы
- Показы и клики
- Превышение бюджета
- Ошибки обработки событий

### Метрики (будущая реализация):
- CTR (Click-Through Rate)
- Расход бюджета по дням
- Количество активных кампаний
- Конверсия по категориям

## 🔐 Безопасность

- JWT аутентификация (в разработке)
- Валидация входных данных
- Проверка прав доступа для создания рекламы
- HTTPS в продакшене

## 🧪 Тестирование

```bash
# Запустить тесты
dotnet test

# Покрытие кода
dotnet test /p:CollectCoverage=true
```

## 📝 Миграции БД

```bash
# Создать миграцию
dotnet ef migrations add InitialCreate --context AdvertisementDbContext

# Применить миграции
dotnet ef database update --context AdvertisementDbContext
```

## 🐛 Обработка ошибок

- Повторные попытки при ошибках RabbitMQ (экспоненциальная задержка)
- Логирование всех исключений
- Graceful degradation при недоступности Redis
- Валидация данных перед сохранением

## 📈 Планы развития

- [ ] A/B тестирование креативов
- [ ] Динамическое ценообразование CPC
- [ ] Геотаргетинг
- [ ] Ретаргетинг пользователей
- [ ] Интеграция с платежными системами
- [ ] Расширенная аналитика
- [ ] GraphQL API

---

**Версия:** 1.0.0  
**Дата обновления:** 2024-01-15  
**Контакт:** support@cadstorage.ru
