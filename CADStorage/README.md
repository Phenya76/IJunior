# 🚀 CADStorage - Маркетплейс инженерных 3D моделей

Полнофункциональная платформа для покупки и продажи STEP файлов и других 3D моделей.

## 📋 Содержание

1. [Архитектура проекта](#архитектура-проекта)
2. [Структура микросервисов](#структура-микросервисов)
3. [Технологический стек](#технологический-стек)
4. [Быстрый старт](#быстрый-старт)
5. [Инструкция по запуску](#инструкция-по-запуску)
6. [API документация](#api-документация)

---

## 🏗️ Архитектура проекта

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (React + Vite)                  │
│              http://localhost:3000 / 5173                   │
└──────────────────────┬──────────────────────────────────────┘
                       │ HTTP/REST API
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                     API Gateway                             │
│              http://localhost:5000                          │
│         (Маршрутизация, аутентификация, rate limiting)      │
└─────┬──────────────┬──────────────┬──────────────┬─────────┘
      │              │              │              │
      ▼              ▼              ▼              ▼
┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────────┐
│   Auth   │  │  Model   │  │ Catalog  │  │Advertisement │
│ Service  │  │ Service  │  │ Service  │  │   Service    │
│ :5001    │  │ :5002    │  │ :5004    │  │   :5003      │
└────┬─────┘  └────┬─────┘  └────┬─────┘  └──────┬───────┘
     │             │             │                │
     └─────────────┴──────┬──────┴────────────────┘
                          │
              ┌───────────┼───────────┐
              │           │           │
              ▼           ▼           ▼
        ┌─────────┐ ┌─────────┐ ┌──────────┐
        │PostgreSQL│ │  Redis  │ │ RabbitMQ │
        │Database │ │  Cache  │ │  Events  │
        └─────────┘ └─────────┘ └──────────┘
```

---

## 📁 Структура микросервисов

### Backend (.NET 8)

```
CADStorage/
├── src/
│   ├── BuildingBlocks/
│   │   └── CADStorage.Common/          # Общие библиотеки
│   │       ├── Entities/               # Базовые сущности
│   │       ├── DTOs/                   # Объекты передачи данных
│   │       ├── Events/                 # События домена
│   │       ├── Interfaces/             # Интерфейсы сервисов
│   │       ├── Exceptions/             # Исключения
│   │       ├── Constants/              # Константы
│   │       ├── Services/               # Общие сервисы
│   │       └── Extensions/             # Методы расширения
│   │
│   └── Services/
│       ├── CADStorage.ApiGateway/      # API Шлюз (порт 5000)
│       ├── CADStorage.Auth.Service/    # Аутентификация (порт 5001)
│       ├── CADStorage.Model.Service/   # Управление моделями (порт 5002)
│       ├── CADStorage.Advertisement.Service/ ← РАЗРАБОТАНО ✅
│       │                                   # Реклама (порт 5003)
│       ├── CADStorage.Catalog.Service/ # Каталог (порт 5004)
│       ├── CADStorage.Search.Service/  # Поиск (порт 5005)
│       ├── CADStorage.Marketplace.Service/ # Покупки (порт 5006)
│       ├── CADStorage.UserProfile.Service/ # Профили (порт 5007)
│       ├── CADStorage.Media.Service/   # Файлы (порт 5008)
│       ├── CADStorage.Notification.Service/ # Уведомления (порт 5009)
│       └── CADStorage.Chat.Service/    # Чат (порт 5010)
│
└── tests/                              # Тесты
```

### Frontend (React + Vite + TypeScript)

```
frontend/
├── src/
│   ├── components/
│   │   └── ui/
│   │       ├── atoms/          # Базовые компоненты
│   │       ├── molecules/      # Составные компоненты
│   │       ├── organisms/      # Сложные компоненты
│   │       └── templates/      # Шаблоны страниц
│   │
│   ├── pages/                  # Страницы приложения
│   ├── hooks/                  # Кастомные хуки
│   ├── contexts/               # React контексты
│   ├── services/               # API клиенты
│   ├── utils/                  # Утилиты
│   └── styles/                 # Глобальные стили
│
├── .storybook/                 # Storybook конфигурация
├── package.json
└── vite.config.ts
```

---

## 💻 Технологический стек

### Backend
- **.NET 8 LTS** - основная платформа
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - основная база данных
- **Redis** - распределенное кэширование
- **RabbitMQ + MassTransit** - шина сообщений
- **JWT** - аутентификация
- **FluentValidation** - валидация данных
- **Swagger/OpenAPI** - документация API

### Frontend
- **React 18** - UI библиотека
- **Vite** - сборщик
- **TypeScript** - типизация
- **TailwindCSS** - стилизация
- **React Router** - маршрутизация
- **Axios** - HTTP клиент
- **Storybook** - документация компонентов
- **React Query** - управление состоянием сервера

### DevOps
- **Docker** - контейнеризация
- **docker-compose** - оркестрация
- **GitHub Actions** - CI/CD

---

## ⚡ Быстрый старт

### Требования

1. **.NET 8 SDK** - [Скачать](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Node.js 18+** - [Скачать](https://nodejs.org/)
3. **Docker Desktop** - [Скачать](https://www.docker.com/products/docker-desktop)
4. **Visual Studio 2022** (русская версия) или **VS Code**

### Установка Docker контейнеров

```bash
# Перейти в корень проекта
cd CADStorage

# Запустить все сервисы (БД, Redis, RabbitMQ)
docker-compose up -d

# Проверить статус
docker-compose ps
```

---

## 📖 Подробная инструкция по запуску

### Шаг 1: Подготовка базы данных

```bash
# Запустить PostgreSQL, Redis и RabbitMQ через Docker
docker run -d --name postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:15

docker run -d --name redis \
  -p 6379:6379 \
  redis:7

docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

### Шаг 2: Запуск микросервиса рекламы

```bash
# Перейти в директорию сервиса
cd CADStorage/src/Services/CADStorage.Advertisement.Service

# Восстановить NuGet пакеты
dotnet restore

# Применить миграции БД
dotnet ef database update

# Запустить сервис
dotnet run --urls=http://localhost:5003
```

**Проверка:**
- Открыть браузер: http://localhost:5003/swagger
- Должен отобразиться Swagger UI

### Шаг 3: Запуск фронтенда

```bash
# Перейти в директорию фронтенда
cd frontend

# Установить зависимости
npm install

# Запустить режим разработки
npm run dev
```

**Проверка:**
- Открыть браузер: http://localhost:3000
- Должна загрузиться главная страница

---

## 📡 API Документация

### Advertisement Service

#### Получить размещения для каталога

**Запрос:**
```http
GET http://localhost:5003/api/advertisement/catalog-placements?modelsCount=20&category=Детали
```

**Ответ:**
```json
[
  {
    "position": 3,
    "advertisement": {
      "id": "a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11",
      "text": "Профессиональные 3D модели по доступным ценам!",
      "imageUrl": "https://cdn.cadstorage.ru/ads/banner1.jpg",
      "clickUrl": "https://cadstorage.ru/models/premium",
      "type": "Contextual"
    }
  },
  {
    "position": 8,
    "advertisement": {
      "id": "b1ffcd88-8d1c-5fg9-cc7e-7cc0ce491b22",
      "text": "Скидка 20% на первую покупку",
      "imageUrl": null,
      "clickUrl": "https://cadstorage.ru/promo",
      "type": "Banner"
    }
  }
]
```

#### Создать рекламную кампанию

**Запрос:**
```http
POST http://localhost:5003/api/advertisement
Content-Type: application/json
Authorization: Bearer {token}

{
  "title": "Продвижение модели двигателя",
  "text": "Высококачественная 3D модель V8 двигателя. Детализация до болтов.",
  "imageUrl": "https://cdn.cadstorage.ru/models/engine-preview.jpg",
  "clickUrl": "https://cadstorage.ru/models/v8-engine-123",
  "type": "ModelPromotion",
  "modelId": "123e4567-e89b-12d3-a456-426614174000",
  "startDate": "2024-01-15T00:00:00Z",
  "endDate": "2024-02-15T23:59:59Z",
  "costPerClick": 15.50,
  "budget": 5000,
  "priority": 10,
  "targetKeywords": ["Двигатели", "Автомобили", "V8"]
}
```

**Ответ:**
```json
{
  "id": "c2ggde77-7e2d-6gh0-dd8f-8dd1df502c33",
  "title": "Продвижение модели двигателя",
  "status": "Draft",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

#### Записать клик

**Запрос:**
```http
POST http://localhost:5003/api/advertisement/{adId}/click
```

**Ответ:** `200 OK`

---

## 🔧 Конфигурация

### Переменные окружения для разработки

Создайте файл `.env` в корне проекта:

```env
# PostgreSQL
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

# JWT Secret
JWT_SECRET=YourSuperSecretKeyForJWTTokenGenerationMustBeLongEnough123!
```

---

## 🧪 Тестирование

### Backend тесты

```bash
# Запустить все тесты
dotnet test

# Запустить с покрытием
dotnet test /p:CollectCoverage=true

# Запустить конкретный тест проект
dotnet test Tests/CADStorage.Advertisement.Service.Tests
```

### Frontend тесты

```bash
cd frontend

# Unit тесты
npm run test

# E2E тесты
npm run test:e2e

# Проверка типов
npm run type-check
```

---

## 📊 Мониторинг и логи

### Логи микросервисов

Логи выводятся в консоль и сохраняются в файлы:

```
logs/
├── advertisement-service.log
├── auth-service.log
└── ...
```

### RabbitMQ Management

Откройте панель управления: http://localhost:15672
- Логин: `guest`
- Пароль: `guest`

### Redis CLI

```bash
docker exec -it redis redis-cli

# Просмотр ключей
KEYS *

# Просмотр значения
GET user:profile:123
```

---

## 🐛 Решение проблем

### Ошибка подключения к базе данных

```bash
# Проверить запущен ли PostgreSQL
docker ps | grep postgres

# Посмотреть логи
docker logs postgres

# Перезапустить
docker restart postgres
```

### Ошибка MassTransit/RabbitMQ

```bash
# Проверить подключение к RabbitMQ
docker exec -it rabbitmq rabbitmqctl list_queues

# Перезапустить RabbitMQ
docker restart rabbitmq
```

### Проблемы с миграциями

```bash
# Удалить последнюю миграцию
dotnet ef migrations remove

# Создать заново
dotnet ef migrations add InitialCreate

# Применить
dotnet ef database update
```

---

## 📈 Планы развития

### Q1 2024
- [x] Создание Common библиотеки
- [x] Микросервис рекламы
- [ ] Микросервис аутентификации
- [ ] Микросервис моделей
- [ ] Базовый фронтенд

### Q2 2024
- [ ] Поисковый сервис (Elasticsearch)
- [ ] Сервис уведомлений (Email, Push)
- [ ] Платежный шлюз
- [ ] 3D просмотрщик (Three.js)

### Q3 2024
- [ ] Мобильное приложение
- [ ] GraphQL API
- [ ] CDN для файлов
- [ ] Аналитика и метрики

---

## 👥 Команда проекта

- **Backend Lead**: Разработчик .NET
- **Frontend Lead**: Разработчик React
- **DevOps**: Инженер инфраструктуры
- **QA**: Тестировщик

## 📞 Контакты

- Email: support@cadstorage.ru
- Telegram: @cadstorage_support
- GitHub: https://github.com/cadstorage

---

**Лицензия:** MIT  
**Версия:** 1.0.0  
**Дата обновления:** 2024-01-15
