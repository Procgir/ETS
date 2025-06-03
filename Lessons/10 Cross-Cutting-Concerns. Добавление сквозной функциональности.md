## Цели и задачи
Сформировать представление о [Чистой архитектуре ](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)(CleanArchitecture), [Предметно-ориентированном подходе ](https://ru.wikipedia.org/wiki/%D0%9F%D1%80%D0%B5%D0%B4%D0%BC%D0%B5%D1%82%D0%BD%D0%BE-%D0%BE%D1%80%D0%B8%D0%B5%D0%BD%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%BD%D0%BE%D0%B5_%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5)([DDD](https://www.youtube.com/watch?v=6_BhdXLxiic)) и освоить разработку с помощью этих подходов на примере Системы Электронного тестирования
- интеграция аутентификации и авторизации с помощью Keycloak
- интегрировать логгирование с помощью Serlilog
- интегрировать кеширование с помощью Redis
- реализовать ApiVersioning (Swagger)
- реализовать HealthChecks подход
- реализовать OutboxPattern для обработки событий с использованием библиотеки Quartz

---
## Теория

**Cross-cutting concerns**  — это аспекты программного обеспечения, которые затрагивают несколько компонентов или слоёв системы, но не входят в основную бизнес-логику. Они часто приводят к дублированию кода и нарушению принципа разделения ответственности.

🔍 Что такое Cross-Cutting Concerns?

Это функциональности, которые необходимо реализовать во многих частях приложения, но которые не относятся напрямую к его основной задаче. Примеры включают:
- **Логирование**
- **Аутентификация и авторизация**
- **Обработка ошибок**
- **Транзакционное управление**
- **Кэширование**
- **Мониторинг и трассировка**
- **Конфигурация и настройка среды

Эти concerns часто приводят к **scattering** (распределению кода по всему приложению) и **tangling** (переплетению функциональности), что затрудняет поддержку и развитие системы.

🛠️ Как управлять Cross-Cutting Concerns?

Для эффективного управления кросс-обеспечивающими проблемами применяются следующие подходы:
- **Аспектно-ориентированное программирование (AOP)**: Позволяет выделить concerns в отдельные модули (аспекты), которые внедряются в нужные места программы.
- **Мидлварь и фильтры**: Используются в веб-приложениях для обработки запросов и ответов (например, логирование, аутентификация).
- **Интерсепторы и декораторы**: Применяются для перехвата вызовов методов и добавления дополнительной логики.
- **Централизованные библиотеки и фреймворки**: Использование общих решений для обработки кросс-обеспечивающих проблем во всей системе.

[Cross-cutting concern — Wikipedia](https://en.wikipedia.org/wiki/Cross-cutting_concern)

### Аутентификация/авторизация
В ASP.NET Core аутентификация и авторизация являются ключевыми аспектами обеспечения безопасности веб-приложений и API. Они позволяют контролировать доступ пользователей к ресурсам и защищать данные.

🔐 Аутентификация в ASP.NET Core

**Аутентификация** — это процесс проверки подлинности пользователя, то есть определение, кто он такой.

Основные схемы аутентификации:

- **Cookie-based Authentication**: Используется для веб-приложений, где после успешного входа пользователя идентифицируют с помощью cookie.
- **JWT (JSON Web Token)**: Применяется в API и SPA-приложениях. После успешной аутентификации сервер выдает клиенту токен, который тот отправляет в заголовке Authorization при последующих запросах.
- **OAuth 2.0 / OpenID Connect**: Используются для интеграции с внешними провайдерами аутентификации, такими как Google, Facebook, Microsoft и другие. [fuse8.ru](https://fuse8.ru/articles/secure-authorization-with-jwt?)

Пример настройки JWT-аутентификации:
```
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

🛡️ Авторизация в ASP.NET Core
**Авторизация** — это процесс определения, какие действия может выполнять аутентифицированный пользователь.

### Основные подходы к авторизации:
- **Ролевая авторизация (Role-based Authorization)**: Ограничивает доступ к ресурсам на основе ролей пользователя.[run-dev](https://run-dev.ru/csharp/autentifikacziya-i-avtorizacziya-api-v-asp-net-core-csharp/?)
- **Авторизация на основе утверждений (Claims-based Authorization)**: Использует утверждения (claims), такие как "email", "age", "permissions", для принятия решений о доступе.[Microsoft Learn+1run-dev+1](https://learn.microsoft.com/ru-ru/aspnet/core/security/authentication/identity?view=aspnetcore-9.0&)
- **Политики авторизации (Policy-based Authorization)**: Предоставляют более гибкий способ определения требований доступа, комбинируя различные утверждения и роли.
- **Авторизация на основе ресурсов (Resource-based Authorization)**: Осуществляется проверка прав доступа к конкретным ресурсам, например, к документам, принадлежащим определенному пользователю.

```
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

В данном примере доступ к контроллеру `AdminController` ограничен пользователями, имеющими роль "Admin".[run-dev](https://run-dev.ru/csharp/autentifikacziya-i-avtorizacziya-api-v-asp-net-core-csharp/?)

🧩 Интеграция аутентификации и авторизации

Для полноценной работы аутентификации и авторизации необходимо настроить соответствующие middleware в конвейере обработки запросов:


```
app.UseAuthentication(); 
app.UseAuthorization();
```

Эти строки обеспечивают проверку подлинности пользователя и оценку его прав доступа перед выполнением запроса.

**Полезные ссылки:**
- [Общие сведения о авторизации в ASP.NET Core](https://learn.microsoft.com/ru-ru/aspnet/core/security/authorization/introduction?view=aspnetcore-9.0)
- [Простая авторизация в ASP.NET Core](https://learn.microsoft.com/ru-ru/aspnet/core/security/authorization/simple?view=aspnetcore-9.0)
- [Введение в Identity на ASP.NET Core](https://learn.microsoft.com/ru-ru/aspnet/core/security/authentication/identity?view=aspnetcore-9.0)

**KeyCloak**
- **Keycloak** — open-source сервер для управления пользователями, аутентификацией и авторизацией по OAuth2/OpenID Connect.
- Asp.Net Core интегрируется с Keycloak через OpenID Connect.
- Настраивается через middleware `AddAuthentication().AddOpenIdConnect()`.
- Keycloak выступает в роли провайдера Identity (Identity Provider, IdP).

**Полезные ссылки:**
- [Официальная документация Keycloak: ](https://www.keycloak.org/documentation)
- [Пример интеграции с Asp.Net Core ](https://medium.com/swlh/asp-net-core-keycloak-integration-with-openid-connect-7a1444463997)
-  [Microsoft Docs по OpenID Connect:](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/openid-connect)
### Логгирование
Логгирование — это процесс записи информации о работе приложения: ошибок, предупреждений, событий, действий пользователя, времени выполнения и т.д. Это помогает:
- Отслеживать поведение приложения в реальном времени
- Диагностировать ошибки и неисправности
- Анализировать производительность
- Вести аудит и обеспечивать безопасность

В ASP.NET Core есть встроенный механизм логгирования — **ILogger**. Он работает через **Dependency Injection**, и ты можешь получить его в любом сервисе, контроллере и т.д.

```
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Index page visited");
        return View();
    }
}
```
**Serilog**
- **Структурированное логгирование.** Serilog умеет логировать данные в структурированном виде — не просто строку, а пару "ключ-значение". Это упрощает фильтрацию и анализ логов.
- **Много сингков (sink).** Sink — это место, куда пишутся логи: консоль, файл, база данных, Seq, ElasticSearch, и др.
- **Простая конфигурация.** Легко настраивается через код и конфиги.
- **Поддержка фильтров и уровней логов.**

**Установка пакетов**
```
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```
Настройка в `Program.cs`
```
`using Serilog;  var builder = WebApplication.CreateBuilder(args);  // Настройка Serilog Log.Logger = new LoggerConfiguration()     .MinimumLevel.Debug()                   // Минимальный уровень логирования     .WriteTo.Console()                      // Логи в консоль     .WriteTo.File("logs/myapp-.log", rollingInterval: RollingInterval.Day) // Логи в файл, ежедневная ротация     .CreateLogger();  builder.Host.UseSerilog(); // Подключаем Serilog как логгер для приложения  var app = builder.Build();  app.MapGet("/", () => "Hello World!");  app.Run();`
```

Использование в контроллерах и сервисах
```
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Запрос к WeatherForecast");
        return Enumerable.Empty<WeatherForecast>();
    }
}
```
Пример кода
```
_logger.LogInformation("User {UserId} logged in at {LoginTime}", user.Id, DateTime.UtcNow);
```
Конфигурация через файл appsettings.json (опционально)
```
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```
Конфигурация
```
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
```
[Serilog Дока](https://serilog.net/)
[Serilog Интеграция в AspNetCore](https://github.com/serilog/serilog-aspnetcore)
[Статья](https://habr.com/ru/articles/550582/)
### Кеширование
Кеширование — это способ **временного хранения часто используемых данных** в быстром доступе, чтобы не делать дорогие операции (например, запрос к базе данных или сложные вычисления) каждый раз заново.

**Пример:**  
Представь, что у тебя есть сайт с информацией о погоде. Каждый раз обращаться к внешнему сервису погоды дорого и долго. Вместо этого ты можешь сохранить последний результат в кеш, и, если запросы приходят часто, отдавать данные из кеша, пока они не устарели.

**Redis** — это популярный **быстрый in-memory (в памяти) key-value хранилище**.  
Он очень часто используется для кеширования, потому что хранит данные в оперативной памяти, что делает доступ к ним очень быстрым.

В ASP.NET Core Redis часто используется для кеширования данных или для сессий, потому что он быстрый и простой в использовании.

**Варианты использования Redis в ASP.NET Core:**

1. **Distributed Cache (распределённый кеш)**  
    ASP.NET Core имеет встроенный интерфейс `IDistributedCache` — абстракцию для кеша, которую можно реализовать с помощью Redis. Это удобно, если у тебя несколько серверов, и кеш нужно сделать общим для всех.
2. **Session Storage**  
    Можно использовать Redis для хранения сессий пользователя — это полезно, когда у тебя несколько серверов и нужно сохранять сессии централизованно.
3. **Кеширование данных в приложении** — например, результаты тяжелых запросов, кэширование страниц или фрагментов.

**Основные особенности Redis:**
- Очень быстрая работа с данными (миллисекунды)
- Поддержка различных структур данных: строки, списки, множества, хеши и др.
- Поддержка TTL (time-to-live) — можно задать время жизни кеша
- Распределённость — можно использовать в кластере

**Как настроить Redis в ASP.NET Core (кратко)**
```
#Пакет
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis

#Конфиги
"Redis": {
  "Configuration": "localhost:6379"
}

#DI
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis")["Configuration"];
});

#Интеграция
public class WeatherService
{
    private readonly IDistributedCache _cache;

    public WeatherService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string> GetWeatherAsync(string city)
    {
        string cacheKey = $"weather_{city}";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            return cachedData; // отдаем кешированные данные
        }

        // Иначе делаем запрос к API погоды (эмуляция)
        string freshData = $"Погода для {city} на {DateTime.Now}";

        // Сохраняем в кеш на 5 минут
        await _cache.SetStringAsync(cacheKey, freshData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return freshData;
    }
}
```

### ApiVersioning (Версионирование API)

Когда у тебя есть публичное API, со временем появляются изменения: добавляются новые возможности, меняется поведение, удаляются устаревшие поля. Чтобы не сломать старые клиенты, полезно **поддерживать несколько версий API**.

Например, версия 1 возвращает данные в одном формате, а версия 2 — немного по-другому.


Существует официальный пакет — **Microsoft.AspNetCore.Mvc.Versioning**. Он позволяет легко добавить поддержку нескольких версий.

 **Основные способы указания версии API:**
- В URL: `/api/v1/products` и `/api/v2/products`
- В HTTP заголовке (например, `api-version: 1.0`)
- В query-параметре (`/api/products?api-version=1.0`)

```
#Пакет
dotnet add package Microsoft.AspNetCore.Mvc.Versioning

#DI
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true; // если версия не указана, используется дефолтная
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true; // в ответе указывает поддерживаемые версии
});

#Пример
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetV1() => Ok(new { Version = "v1", Products = new string[] { "Apple", "Banana" } });
}

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetV2() => Ok(new { Version = "v2", Products = new string[] { "Apple", "Banana", "Cherry" } });
}
```
Для версионирование со Swagger настраивают генерацию документов по версиям.

Пример настройки Swagger для версионированного API:
```
builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
                       .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = $"My API {description.ApiVersion}",
            Version = description.ApiVersion.ToString()
        });
    }
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // например, v1, v1.0
    options.SubstituteApiVersionInUrl = true;
});

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});
```
### HealthChecks
Health Checks — это механизм, который позволяет проверять, в рабочем ли состоянии твое приложение и его зависимости (например, база данных, кеш, сторонние сервисы).

Это важно, чтобы:
- Быстро обнаружить проблемы в продакшене
- Автоматизировать мониторинг и алерты
- Поддерживать высокую доступность и устойчивость

Ты регистрируешь набор проверок (health checks), которые запускаются по запросу на специальный endpoint (обычно `/health`).

Пример: проверка подключения к базе данных, Redis, доступности стороннего API.

```
dotnet add package Microsoft.AspNetCore.Diagnostics.HealthChecks

builder.Services.AddHealthChecks()
    .AddSqlServer(configuration.GetConnectionString("DefaultConnection"))  // проверка базы
    .AddRedis(configuration["Redis:Configuration"])                       // проверка Redis
    .AddUrlGroup(new Uri("https://externalapi.com/health"));              // проверка внешнего API

app.MapHealthChecks("/health");
```
Кастомная проверка
```
public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        bool isHealthy = true; // твоя логика проверки

        if (isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Everything is OK"));
        }
        else
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Something went wrong"));
        }
    }
}
```
### OutboxPattern
**Outbox Pattern** — это шаблон проектирования, который помогает гарантировать **надежную передачу сообщений или событий из одной системы (например, базы данных) в другую (например, очередь сообщений)**, даже при сбоях.

Представь ситуацию:
- У тебя есть операция, которая изменяет данные в базе.
- После успешного изменения нужно отправить событие или сообщение в очередь (RabbitMQ, Kafka и т.п.).

Если просто изменить данные и отдельно отправить сообщение, то:
- Может произойти сбой между этими двумя действиями — например, данные изменились, а сообщение не отправилось, или наоборот.
- В итоге данные и сообщения **несогласованы** — это называется **неатомарной операцией**.

**Outbox Pattern** решает эту проблему.

Идея проста:
1. Вместо того, чтобы сразу отправлять событие в очередь, сохраняем событие **в специальную таблицу (Outbox)** внутри той же транзакции, где меняем основные данные.
2. Эта транзакция гарантирует, что либо и данные, и событие сохранятся, либо ничего не сохранится.
3. Отдельный процесс или служба периодически читает сообщения из таблицы Outbox и отправляет их в очередь. После успешной отправки — помечает или удаляет записи из Outbox

```
[Транзакция базы]
  - Обновить бизнес-данные
  - Записать событие в таблицу Outbox
[Отдельный процесс]
  - Читает события из Outbox
  - Отправляет их в очередь сообщений
  - Помечает событие как отправленное
```
**Преимущества**
- **Атомарность**: данные и события сохраняются в одной транзакции.
- **Надежность**: сообщения точно не потеряются даже при сбоях.
- **Отделение логики**: отправка сообщений вынесена в отдельный процесс, что упрощает обработку ошибок и повторные попытки.

Есть несколько подходов:
Ручная реализация
- Создаёшь таблицу Outbox в базе данных (например, SQL Server).
- В коде сервиса, когда меняешь данные, в той же транзакции добавляешь запись в Outbox.
- Пишешь фоновый сервис (Hosted Service), который периодически читает и отправляет сообщения.

Использование библиотек и фреймворков
- **NServiceBus** — очень популярный инструмент, который реализует Outbox автоматически.
- **[CAP (dotnetcore/CAP)](https://github.com/dotnetcore/CAP)** — open-source решение для обработки сообщений и Outbox pattern.
- **MassTransit** тоже поддерживает outbox-модель.

```
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsSent { get; set; }
}

using var transaction = await dbContext.Database.BeginTransactionAsync();

// Меняем бизнес-данные
dbContext.Orders.Add(newOrder);

// Добавляем событие в Outbox
dbContext.OutboxMessages.Add(new OutboxMessage
{
    Id = Guid.NewGuid(),
    Type = "OrderCreated",
    Payload = JsonSerializer.Serialize(orderDto),
    CreatedAt = DateTime.UtcNow,
    IsSent = false
});

await dbContext.SaveChangesAsync();
await transaction.CommitAsync();

public async Task SendOutboxMessages()
{
    var unsentMessages = await dbContext.OutboxMessages
        .Where(m => !m.IsSent)
        .ToListAsync();

    foreach (var message in unsentMessages)
    {
        // Отправляем в очередь (RabbitMQ, Kafka и т.п.)
        await messageSender.SendAsync(message.Type, message.Payload);

        message.IsSent = true;
    }

    await dbContext.SaveChangesAsync();
}
```

https://dev.to/christianzink/the-outbox-pattern-in-event-driven-asp-net-core-microservice-architectures-89
### Дополнительные материалы
[Metanit полное руководство по C#](https://metanit.com/sharp/tutorial/1.1.php)
[Metanit полное руководство по Asp.Net Core](https://metanit.com/sharp/aspnet6/)

[Рефакторинг гуру](https://refactoring.guru/ru/refactoring)
[Паттерны проектирования](https://refactoring.guru/ru/design-patterns)

[Паттерный проектирования](https://codelibrary.info/books/c-sharp/patterny-proektirovaniya-dlya-c-i-platformy-net-core?highlight=WyJjIl0=)
[Рефакторинг Улучшение существующего кода](https://codelibrary.info/books/dlya-programmistov/refaktoring)

[Простое объектно-ориентированное проектирование](https://codelibrary.info/books/dlya-programmistov/prostoe-obektno-orientirovannoe-proektirovanie)
[Проектирование API](https://codelibrary.info/books/dlya-programmistov/proektirovanie-arkhitektury-api)
[Искусство чистого кода](https://codelibrary.info/best-books/c-sharp?highlight=WyJjIl0=)

[Архитектурные паттерны](https://github.com/RefactoringGuru/design-patterns-csharp)
#### Полезные ссылки
[Дорожные карты Backend](https://roadmap.sh/backend)
[Дорожная карта Asp.Net Core](https://roadmap.sh/aspnet-core)
[Алгоритмы](https://neetcode.io/roadmap)

https://www.youtube.com/watch?v=w8rRhAup4kg
https://www.youtube.com/watch?v=_8yZYhAkQjQ
https://www.youtube.com/watch?v=pFKwmEdwZZQ
https://neetcode.io/roadmap

---
## Практика
Цель - научиться реализовывать CCC в простых веб-сервисах
- интеграция аутентификации и авторизации с помощью Keycloak
- интегрировать логгирование с помощью Serlilog
- интегрировать кеширование с помощью Redis
- реализовать ApiVersioning (Swagger)
- реализовать HealthChecks подход
- реализовать OutboxPattern для обработки событий с использованием библиотеки Quartz

---
## Упражения
Продолжаем реализовывать Систему Электронного тестирования. Для реализации аутентификации и авторизации используем сервис идентификации KeyCloak и реализуем методы, которые позволят регистрировать и авторизовывать пользователей в нашем приложении
### 1
Добавьте сервис идентификации KeyCloak с помощью, которого будем производить создание и аутентификацию/авторизацию пользователя и интегрируйте аутентификацию в API нашего приложения
- добавьте в докер композ файл новый сервис
- добавьте файл конфигурации для работы с KeyCloak
- добавьте пользователя в админке KeyCloak. test@test.com и попробуйте получить токен доступа. Импортируйте настройки запросов для Postman. Файл импорта лежит в корне
- добавьте middleware аутентификации и авторизации в приложение
- добавьте настройки для взаимодействия с KeyCloak, которые в дальнейшем будут использоваться для отправки запросов на регистрацию (создание) пользователей и их аутентификацию/авторизацию
  ![[Pasted image 20250520180818.png]]
### 2
Добавьте функционал регистрации и аутентификации пользователей
- реализуйте регистрацию пользователей
- реализуйте аутентификацию пользователей. Метод Login
- реализуйте метод для получения информации об аутентифицированном пользователе GetLoginUser

3
Добавьте авторизацию пользователей на основе Ролей. Для этого создайте роль Registered для зарегистрированных пользователей и реализуйте логику авторизации вокруг нее
- добавьте авторизацию на основе типов Доступов 
- добавьте авторизацию на основе доступа к ресурсам  

4
Добавьте авторизацию пользователей на основе основе типов Доступов. Для этого создайте политику доступа "users:read"

5
Добавьте авторизацию пользователей на основе доступа к ресурсам. Для этого прокидывайте авторизованного пользователя в контроллеры, где они требуются и добавьте проверку на принадлежность ресурса пользователю. На примере получения тестов

6
Добавьте сервис логгирования Serilog, который позволит хранить и анализировать логи
- добавьте новый сервис в докер композ
- подключите Serilog к API и протестируйте его работы: отправьте запросы и посмотрите логи, которые пишутся в сервис логов
- по одному запросу может быть много логов и сейчас они в одной куче. Для того, чтобы можно было отделять логи разных запросов друг от друга обогатите их с помощью CorrelationId. Это позволит видеть цепочку логов одного запроса и фильтровать логи только для него
- Доработайте логгирование на уровне приложения: все запросы логгируются (не только команды), неудачный результат обрабатывается и ошибка добавляется в лог

7
В приложении есть неоптимальные места запроса различных данных: получение ролей/доступов пользователя, получение токенов от сервиса Идентификации. Эти данные не изменяются на большом промежутке времени, но мы их постоянно запрашиваем и создаем тем самым дополнительную нагрузку, увеличиваем время выполнения запроса и т.д. 
Поэтому добавим в приложение кеширование, которое позволит решить данные проблемы.
- добавьте сервис кеширования Redis
- интегрируйте работу с кешем в приложение
- доработайте AuthorizationService так, чтобы получаемые роли и доступы кешировались
- доработайте слой приложения так, чтобы можно было из коробки подключать кеширование запросов с помощью добавления в пайплайн нового поведения для Кеширования

8
Чтобы быть всегда уверенным, что ваше приложение работает необходимо знать о его состоянии. Для этого добавьте в API так называемые healthcheck. Методы, которые возвращают информацию о состоянии вашего сервиса
- добавьте в общий пайплайн запроса мидлвару для healthcheck
- настройте кастомный healthcheck, который будет возвращать состояние СУБД, приложения и других сервисов
- настройте healthcheck на основе готовых библиотек добавив библиотеку AspNetCore.HealthChecks.* и добавив в пайплайн приложения

9
В процессе разработки приложение эволюционирует и претерпевает изменения. Текущие методы могут устареть и ждать рефакторинга или удаления. Но с API, которое является публичным дело обстоит иначе. В случае, если работа нашего API меняется, то мы не можем просто так поменять логику. Кто-то уже ей может пользоваться и наши изменения могут поломать работу этих клиентов. Для этого существует версионирование API. Чтобы можно было поднять версию и старую обозначить устаревшей, чтобы с нее перешли на новую. Добавьте версионирование в приложение
- добавьте версионирование API
- добавьте 1 и 2 верисю для UsersController метода GetLoggedIn
- добавьте дополнительные настройки для OpenApi Swagger, чтобы он корректно отображал версию API для всех контроллеров

10
Ранее мы настраивали публикацию доменных событий. Данное решение содержит недостаток: если в процессе публикации произойдет ошибка, то данное событие не будет обработано и согласованность процесса будет нарушена. Для того, чтобы этого избежать используем паттерн OutboxMessage. Для этого мы все создаваемые события будем класть в базу данных в рамках транзакции. А затем в фоновом процессе поочередно их публиковать, что позволит нам решить существующий недостаток.
- создайте модель данных, которая будет содержать в себе создаваемые события OutboxMessage
- переделайте публикацию доменных событий на их сохранение в БД
- добавьте библиотеку Quartz для работы с фоновыми задачами и реализуйте фоновую обработку публикации доменных событий

---
## FAQ
Если у вас возникли вопросы по занятию или общие обратитесь к [FAQ](https://docs.google.com/spreadsheets/d/1_n-wfeDpjv3-NcWxreu7minH0JQ-ooQb9B1KtNMU0eI/edit?usp=sharing)
Возможно ответ уже находится там

Если ответа на вопрос там нет. Напишите его и я обязательно дам ответ

**Инструкция**:
1. Запросите права доступа к таблице, если у вас нет доступа
2. Если вопрос по конкретному занятию, тогда зайдите во вкладку с **названием занятия**, по которому у вас возник вопрос, иначе зайдите на вкладку **Общие**
3. Напишите Вопрос в столбце **Вопрос**
4. Напишите Фамилию Имя в столбце **Автор**
5. Повторите действия
	1. Укажите Статус Ожидает 
	2. Дождитесь ответа. У вопроса будет статус Ответ дан
	3. Если ответ требует уточнения перевидте вопрос в статус Требует уточнения и напишите комментарий в столбце Комментарий
	4. Повторите, пока не получите ответ на свой вопрос
6. Переведите ответ в статус Завершен
7. Если ответ вам помог разобраться, увеличьте число в колонке **Лайки**, это поможет понять насколько эти вопросы общие и вынести ответы в материалы курса

Спасибо, что оставили вопрос, благодаря этому курс может стать лучше

---
## Обратная связь
**Обязательная обратная связь**
Для автоматизации процесса контроля за работой учеников и улучшения курса необходимо **обязательно** оставить [обратную связь по курсу](https://docs.google.com/forms/d/e/1FAIpQLSfOZTyNNNA-GDpWapoh7g_fmHYYpO8_1ZnoWsDyQTubAHvrFw/viewform?usp=dialog)
Обратная связь обязательно с точки зрения факта отправки, но нет цели заставить вас что-то писать в ней, если вам нечего сказать.

**Необязательная обратная связь** (АНОНИМНАЯ)
Если вам есть, что сказать по занятиям, но вы по каким-то причинам не готовы этим поделиться в обязательной форме обратной связи, то буду очень благодарен, если оставите ее в [анонимной обратной связи](https://docs.google.com/forms/d/e/1FAIpQLSfvVEkllf7gLIlkZLLwKXgkLVbj2sHrX4wMT4dcCeICE-K1rQ/viewform?usp=dialog)
