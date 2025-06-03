## Цели и задачи
Сформировать представление о [Чистой архитектуре ](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)(CleanArchitecture), [Предметно-ориентированном подходе ](https://ru.wikipedia.org/wiki/%D0%9F%D1%80%D0%B5%D0%B4%D0%BC%D0%B5%D1%82%D0%BD%D0%BE-%D0%BE%D1%80%D0%B8%D0%B5%D0%BD%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%BD%D0%BE%D0%B5_%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5)([DDD](https://www.youtube.com/watch?v=6_BhdXLxiic)) и освоить разработку с помощью этих подходов на примере Системы Электронного тестирования
- реализовать слой инфраструктуры в подходе CA + CQRS + DDD
- реализовать слой Представления в подходе CA + CQRS + DDD
- интегрировать работу с базой данных PostgreSQL в приложение с использование EntityFramework Core и наполнить ее тестовыми данными с помощью библиотеки Bogus
- сформировать практические навыки работы с клиентами для управления СУБД Dbeaver
- настроить запуск сервиса WebApi в Docker контейнере
- интегрировать OpenApi Swagger
- реализовать глобальную обработку ошибок 

---
## Теория
**Clean Architecture** и **DDD** — это подходы, направленные на создание гибких, легко масштабируемых и сопровождаемых систем. Они позволяют отделить бизнес-логику от инфраструктурных и внешних зависимостей, что облегчает тестирование, поддержку и развитие приложения.

### Clean Architecture
Clean Architecture (чистая архитектура) — это подход к проектированию приложений, где бизнес-логика не зависит от фреймворков, UI или БД.

**Основные слои (от внутренних к внешним):**

1. **Domain (ядро)** – бизнес-логика и entities.
2. **Application (use cases)** – сценарии взаимодействия (например, `RegisterUserUseCase`).
3. **Infrastructure** – реализация внешних сервисов (БД, API, кэш).
4. **Presentation** – UI, API-контроллеры.

**Принципы:**
- **Зависимости направлены внутрь** (Domain не знает об Infrastructure).
- **Абстракции важнее реализаций** (интерфейсы репозиториев в Domain, реализация — в Infrastructure).
- **Тестируемость** (бизнес-логику можно тестировать без БД и фреймворков).

**Пример структуры:**
```
/src  
  /Domain          // User.cs, IUserRepository  
  /Application     // CreateUserCommand.cs, UserService  
  /Infrastructure  // UserRepository (EF Core), EmailService  
  /Presentation    // UserController (Web API)  
```
Слои Infrastructure и Presentation являются внешними уровнями архитектуры приложения. 
#### Слой Инфраструктуры
Задача:  отвечает за технические детали: работа с БД, реализация репозиториев, конфигурация, отправка почты, логирование, внешними сервисами. Реализует интерфейсы из Application и Domain.

- Реализация `DbContext`, репозиториев
- Настройка конфигурации
- Подключение к БД (например, EF Core и Dapper)
- Обработка исключений (например, concurrency)
- Публикация доменных событий
- Работа с файлами, почтой, внешними сервисами

Если Domain и Application — это мозг, то Infrastructure — руки и ноги, выполняющие реальные действия.

Infrastructure реализует всё то, что окружает бизнес-логику, но не влияет на её правила. Он может быть легко заменён, не нарушая остальную систему. Зависит от внутренних слоёв, но сам не используется ими напрямую.

##### Подключение ORM
ORM (Object-Relational Mapping) - технология, которая связывает объекты в коде (например, классы C#) с таблицами в БД. Используется для работы с БД как с объектами, без прямых SQL-запросов, что позволяет автоматизировать рутинные операции (CRUD).

###### **Dapper**
Dapper — это микро-ORM, который расширяет `IDbConnection` (обычно `SqlConnection`) для работы с БД. Он не требует сложной настройки, работает быстро и позволяет выполнять SQL-запросы с минимальным оверхедом.

Плюсы:
- Быстрый (почти как ручной SQL).
- Простой (мало "магии").
- Контроль над запросами.
Минусы:
- Нужно писать SQL вручную.
- Нет миграций, ленивой загрузки.

Установка Dapper c PostgreSQL
Установите пакет через NuGet: 
```
Install-Package Dapper
```
Или через .NET CLI: 
```
dotnet add package Dapper
dotnet add package Npgsql
```
Dapper работает с любым `IDbConnection`, но чаще всего используется `SqlConnection` для MS SQL Server.
**Пример**:
```
using Npgsql;
using Dapper;

// Строка подключения (можно хранить в appsettings.json)
string connectionString = "Host=localhost;Database=testdb;Username=postgres;Password=password";

// Создаем подключение
using (var connection = new NpgsqlConnection(connectionString))
{
    connection.Open(); // Dapper сам открывает соединение, но можно и вручную

    // Здесь выполняем запросы...
}
```
Реализация через интерфейс
```
public interface ISqlConnectionFactory {
    IDbConnection CreateConnection();
}

public class SqlConnectionFactory : ISqlConnectionFactory {
    private readonly string _connectionString;
    public SqlConnectionFactory(IConfiguration configuration) {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
```
Пример использования в репозитории
```
public class UserReadRepository {
    private readonly ISqlConnectionFactory _factory;
    public UserReadRepository(ISqlConnectionFactory factory) {
        _factory = factory;
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync() {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<User>("SELECT * FROM Users WHERE IsActive = 1");
    }
}
```
**Основные операции с Dapper**
1Запрос данных (Query)
Получение списка объектов из БД:

```
var users = connection.Query<User>("SELECT * FROM Users WHERE Age > @Age", new { Age = 18 });
```
- `User` — класс C#, соответствующий таблице в БД.
- `@Age` — параметризованный запрос (защита от SQL-инъекций).

2Получение одной записи (QueryFirst, QueryFirstOrDefault)
```
// Если запись обязательна (иначе исключение)
var user = connection.QueryFirst<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = 1 });

// Если записи может не быть (вернет null)
var user = connection.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = 999 });
```
3Вставка и обновление
```
var newUser = new User { Name = "Alex", Age = 25 };
int rowsAffected = connection.Execute(
    "INSERT INTO Users (Name, Age) VALUES (@Name, @Age)", 
    newUser
);

// UPDATE
int updatedRows = connection.Execute(
    "UPDATE Users SET Age = @Age WHERE Id = @Id", 
    new { Age = 30, Id = 1 }
);

// DELETE
int deletedRows = connection.Execute(
    "DELETE FROM Users WHERE Id = @Id", 
    new { Id = 2 }
);
```
Пример кода
```
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<User> GetUsersOverAge(int age)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            return connection.Query<User>(
                "SELECT * FROM Users WHERE Age > @Age", 
                new { Age = age }
            );
        }
    }

    public void AddUser(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Execute(
                "INSERT INTO Users (Name, Age) VALUES (@Name, @Age)", 
                user
            );
        }
    }
}
```
**Плюсы Dapper:**

- Очень быстрый (почти как чистый ADO.NET).
- Минимальный оверхед.
- Полный контроль над SQL.

**Минусы:**

- Нужно писать SQL вручную.
- Нет автоматических миграций.
- Нет ленивой загрузки (как в EF Core).

**Советы:**
1. **Используйте параметризованные запросы** (защита от SQL-инъекций).
2. **Закрывайте соединение** (`using` или `connection.Dispose()`).
3. **Для сложных запросов** лучше писать SQL вручную, чем использовать LINQ-to-SQL.
4. **Оптимизируйте запросы** (например, не выбирайте `SELECT *`, если не нужно).

	
###### **EF Core (Entity Framework Core)**
**EF Core** — это объектно-реляционный маппер (ORM) от Microsoft, который позволяет работать с базами данных, используя объекты C# вместо SQL. Он поддерживает **SQL Server, PostgreSQL, MySQL, SQLite** и другие СУБД.

**Основные концепции**
**1 DbContext**
- Главный класс, который представляет сессию с БД.
- Отслеживает изменения сущностей.
- Выполняет запросы и сохраняет данные.

```
public class AppDbContext : DbContext {
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
```
**Пример**:
Строка подключения в `appsettings.json`:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AppDb;User Id=sa;Password=Your_password123;"
  }
}
```
DbSet - представляет коллекцию сущностей (например, таблицу в БД)
**2 Основные операции**
```
using var db = new AppDbContext();

var user = new User { Name = "Alice", Age = 25 };
db.Users.Add(user); // Добавляем в DbSet
db.SaveChanges();   // Сохраняем в БД
```
```
// Получить всех пользователей
var users = db.Users.ToList();

// Найти по ID
var user = db.Users.Find(1); // WHERE Id = 1

// Фильтрация (LINQ)
var adults = db.Users.Where(u => u.Age >= 18).ToList();
```
```
var user = db.Users.Find(1);
user.Name = "Bob";
db.SaveChanges();
```
```
var user = db.Users.Find(1);
db.Users.Remove(user);
db.SaveChanges();
```
**3 Миграции**
Миграции позволяют обновлять схему БД без ручного написания SQL.
- Создание миграции
```
  dotnet ef migrations add InitialCreate
```
- Применение миграций
```
dotnet ef database update
```
- Откат миграции
```
dotnet ef database update PreviousMigrationName
```
**4 Связи между таблицами (Relationships)**
- Один ко многим (1:N)
```
- public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Post> Posts { get; set; } // У пользователя много постов
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int UserId { get; set; } // Внешний ключ
    public User User { get; set; }  // Навигационное свойство
}
```
- Многие ко многим (N:N)
```
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Course> Courses { get; set; }
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Student> Students { get; set; }
}

// В DbContext:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Student>()
        .HasMany(s => s.Courses)
        .WithMany(c => c.Students);
}
```
**5 Запросы (LINQ vs SQL)**
-  LINQ
```
  var users = db.Users
    .Where(u => u.Age > 18)
    .OrderBy(u => u.Name)
    .ToList();
```
- SQL
```
  var users = db.Users
    .FromSqlRaw("SELECT * FROM Users WHERE Age > 18")
    .ToList();
```
**6 Механизм отслеживания изменений (Change Tracking)**
EF Core автоматически отслеживает изменения в сущностях, которые:

- Были получены из базы данных (если не использовался `AsNoTracking`)
- Были добавлены в контекст через `Add`, `Attach` и т.д.
```
var user = db.Users.First(); // Отслеживается
user.Name = "New Name";     // Изменение фиксируется
db.SaveChanges();           // UPDATE в БД
```
Каждая сущность имеет состояние (`EntityState`):
- **`Added`** — новая сущность (будет `INSERT`).
- **`Modified`** — изменена (будет `UPDATE`).
- **`Deleted`** — помечена на удаление (будет `DELETE`).
- **`Unchanged`** — не изменена.
- **`Detached`** — не отслеживается.
```
var user = new User { Id = 1, Name = "Test" };
db.Entry(user).State = EntityState.Modified; // Принудительно помечаем как изменённую
db.SaveChanges(); // Выполнит UPDATE
```
Отключение отслеживания (AsNoTracking) - если данные только читаются, отслеживание можно отключить для повышения производительности:
```
var users = db.Users.AsNoTracking().ToList(); // Сущности не отслеживаются
```
**Явное отслеживание (`Attach`, `Update`)**
- **`Attach`** — начинает отслеживать сущность (состояние `Unchanged`).
- **`Update`** — начинает отслеживать сущность (состояние `Modified`).
```
var user = new User { Id = 1, Name = "Updated" };
db.Users.Attach(user); // Начинает отслеживать (Unchanged)
user.Name = "New Name"; // Теперь Modified
db.SaveChanges(); // UPDATE
```
**7 Загрузка связанных данных**
**Ленивая загрузка (Lazy Loading)**
- Данные загружаются автоматически при обращении к навигационному свойству.
- Требует:
    - Установки `Microsoft.EntityFrameworkCore.Proxies`.
    - Виртуальных свойств (`virtual`).
```
protected override void OnConfiguring(DbContextOptionsBuilder options)
{
    options.UseLazyLoadingProxies()
           .UseNpgsql("ConnectionString");
}
public class User
{
    public int Id { get; set; }
    public virtual List<Post> Posts { get; set; } // Виртуальное свойство
}

var user = db.Users.First();
var posts = user.Posts; // Автоматически загружает посты (отдельный SQL-запрос)
```
➡ **Плюсы:**
- Удобно, не нужно явно указывать загрузку.
➡ **Минусы:**
- **N+1 проблема** (много запросов к БД).
- Требует `virtual` и дополнительного пакета.
**Ранняя загрузка (Eager Loading)**
- Данные загружаются сразу одним запросом с `Include`.
- Оптимально для предсказуемой загрузки.
```
var users = db.Users
    .Include(u => u.Posts) // JOIN с Posts
    .ToList();
    
var users = db.Users
    .Include(u => u.Posts)
        .ThenInclude(p => p.Comments) // Посты + комментарии
    .ToList();
```
➡ **Плюсы:**
- Минимизация запросов к БД.
- Предсказуемость.
➡ **Минусы:**
- Может загружать лишние данные.

**Явная загрузка (Explicit Loading)**
- Данные загружаются явно через `Load()`.
```
var user = db.Users.First();
db.Entry(user)
  .Collection(u => u.Posts)
  .Load(); // Явная загрузка постов
```

| **Стратегия** | **Когда использовать**               | **Плюсы**           | **Минусы**                 |
| ------------- | ------------------------------------ | ------------------- | -------------------------- |
| **Ленивая**   | Для простых сценариев                | Удобство            | N+1 проблема, тормозит     |
| **Ранняя**    | Когда заранее известны нужные данные | Оптимальные запросы | Может загружать лишнее     |
| **Явная**     | Для динамической загрузки            | Гибкость            | Требует ручного управления |

**Оптимизация загрузки**

**`Select` для проекций**
```
var users = db.Users
    .Select(u => new { u.Name, PostCount = u.Posts.Count })
    .ToList(); // Только нужные поля
```
**Разделение запросов (`Split Queries`)**
```
var users = db.Users
    .Include(u => u.Posts)
    .AsSplitQuery() // Разделяет на несколько SQL-запросов
    .ToList();
```

**8 EntityConfiguration (Конфигурация сущностей)**
Конфигурация сущностей в EF Core позволяет детально настроить маппинг между объектами C# и таблицами БД. Есть два основных подхода:
1. **Data Annotations** - атрибуты над классами и свойствами
2. **Fluent API** - конфигурация через DbContext.OnModelCreating

```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>(entity => 
    {
        entity.ToTable("Users");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Name).HasMaxLength(100);
        entity.HasMany(u => u.Posts).WithOne(p => p.User);
    });
}
```

**Expression Trees (Деревья выражений)**
EF Core преобразует LINQ-запросы в SQL с помощью деревьев выражений:

**IQueryable**
Интерфейс, позволяющий строить запросы с отложенным выполнением:

```
IQueryable<User> query = db.Users;
query = query.Where(u => u.Age > 18);
query = query.OrderBy(u => u.Name);
var result = query.ToList(); // SQL выполняется здесь
```
**DbConcurrencyException (Конфликты параллелизма)**
Возникает при оптимистичной блокировке:

```
try 
{
    db.SaveChanges();
}
catch (DbUpdateConcurrencyException ex)
{
    // Обработка конфликта
}
```
**Репозитории (Repositories)**
Паттерн репозитория абстрагирует работу с EF Core:

```
public interface IRepository<T> where T : class
{
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    IEnumerable<T> GetAll();
}

public class UserRepository : IRepository<User>
{
    private readonly AppDbContext _context;
    
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    // Реализация методов
}
```
**Оптимистическая блокировка (Optimistic Concurrency)**
Реализуется через:
1. Таймстампы (Timestamp)
2. RowVersion
3. Сравнение оригинальных значений

Пример:
```
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    [Timestamp]
    public byte[] Version { get; set; }
}
```
```
Публикация доменных событий (Publishing Domain Events)
```
Паттерн для обработки побочных эффектов:

```
public abstract class Entity
{
    private List<IDomainEvent> _events = new();
    
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();
    
    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);
    
    public void ClearEvents() => _events.Clear();
}

// В DbContext:
public override int SaveChanges()
{
    var entities = ChangeTracker.Entries<Entity>()
        .Select(e => e.Entity)
        .Where(e => e.Events.Any());
    
    var result = base.SaveChanges();
    
    foreach (var entity in entities)
    {
        foreach (var @event in entity.Events)
        {
            _eventDispatcher.Publish(@event);
        }
        entity.ClearEvents();
    }
    
    return result;
}
```

**Преимущества:**
- Работа с БД через объекты (не нужно писать SQL).
- Автоматические миграции.
- Поддержка транзакций, кеширования.

**Недостатки:**
- Медленнее, чем Dapper.
- Может генерировать неоптимальные SQL-запросы.

**EF Core** — это как автоматическая коробка передач: удобно, абстрактно, но иногда не хватает контроля.  
**Dapper** — это как ручная коробка: ты сам всё контролируешь, но нужен больше код.
#### Слой представления
Задача: отвечает за входные точки взаимодействия пользователя с системой: API, UI, CLI. В .NET это слой контроллеров (ASP.NET Core Web API).

Представление — это интерфейс автомобиля: руль, педали, приборная панель.

**1 Компоненты AspNet. Core
(В НАШЕМ СЛУЧАЕ МЫ НЕ ТРОГАЕМ VIEW)
**MVC (Model-View-Controller)**

**Контроллеры (Controllers)**
Контроллеры — это классы, которые:
- Обрабатывают HTTP-запросы
- Возвращают HTTP-ответы
- Связывают маршруты (URL) с логикой приложения
    
**Создание контроллера**
```

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(_service.GetAll());
    }
}
```

**Ключевые атрибуты**

|Атрибут|Назначение|
|---|---|
|`[Route]`|Определяет URL-шаблон для контроллера/метода|
|`[HttpGet]`|Связывает метод с HTTP GET запросом|
|`[HttpPost]`|Связывает метод с HTTP POST запросом|
|`[FromBody]`|Указывает, что параметр берется из тела запроса|
|`[FromQuery]`|Указывает, что параметр берется из query string|
|`[Authorize]`|Требует аутентификации|
|`[ProducesResponseType]`|Документирует возможные коды ответа для Swagger|

**Возвращаемые типы**
- `ActionResult<T>` — стандартный тип для API
- `IActionResult` — когда возвращаемый тип может варьироваться
- `ViewResult` — для MVC (возврат представления)

**Middleware (ПО промежуточного слоя)**
Конвейер обработки запроса

Middleware — это компоненты, которые:
- Обрабатывают входящие запросы и исходящие ответы
- Выполняются последовательно в порядке регистрации
- Могут прервать обработку запроса

```
Запрос → Middleware1 → Middleware2 → Контроллер → Middleware2 → Middleware1 → Ответ
```
```
public void Configure(IApplicationBuilder app)
{
    app.UseHttpsRedirection();       // Перенаправление на HTTPS
    app.UseStaticFiles();            // Обслуживание статических файлов
    app.UseRouting();                // Маршрутизация
    app.UseAuthentication();         // Аутентификация
    app.UseAuthorization();          // Авторизация
    app.UseEndpoints(endpoints =>    // Определение конечных точек
    {
        endpoints.MapControllers();
    });
}
```

**Создание кастомного middleware**

**Способ 1: Класс middleware**
```
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
        await _next(context);
        _logger.LogInformation($"Response: {context.Response.StatusCode}");
    }
}

// Регистрация
app.UseMiddleware<RequestLoggingMiddleware>();
```

**Способ 2: Инлайн middleware**
```

app.Use(async (context, next) =>
{
    // Логика до вызова следующего middleware
    await next();
    // Логика после вызова следующего middleware
});
```

**Dependency Injection (Внедрение зависимостей)**

**Регистрация сервисов**
```

public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IProductService, ProductService>(); // Новый экземпляр для каждого запроса
    services.AddScoped<ICartService, CartService>();         // Один экземпляр на запрос
    services.AddSingleton<ILogger, FileLogger>();            // Один экземпляр на все приложение
}
```
**Внедрение в контроллеры**
```

public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService service,
        ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }
}
```

**Маршрутизация (Routing)**

```
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        // ...
    }
}
```

**Модель приложения ASP.NET Core**

**Жизненный цикл запроса**
1. **Получение запроса** (Kestrel/IIS)
2. **Middleware pipeline** (аутентификация, логирование и т.д.)
3. **Маршрутизация** (выбор контроллера и метода)
4. **Выполнение действия контроллера**
5. **Формирование ответа**
6. **Обратный проход через middleware**

Программная точка входа
```

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

 **Глобальный обработчик исключений**

```
app.UseExceptionHandler("/error");

// В контроллере
[Route("/error")]
public IActionResult HandleError() 
    => Problem(detail: "An unexpected error occurred");


public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) {}
}

// В middleware
catch (NotFoundException ex)
{
    context.Response.StatusCode = 404;
    await context.Response.WriteAsync(ex.Message);
}
```
**2 Docker и окружение разработки**
[Настройка](https://learn.microsoft.com/ru-ru/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-9.0)
[Статья](https://tproger.ru/articles/kak-nastroit-rabotu-net-prilozhenija-sql-server-flyway-migracij-s-pomoshhju-docker-docker-compose)
[Microsoft Blog](https://learn.microsoft.com/ru-ru/dotnet/core/docker/build-container?tabs=linux)
**Docker Images**
- Неизменяемые шаблоны для создания контейнеров
- Собираются через Dockerfile

Пример Dockerfile для ASP.NET Core
```
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

**Docker Compose**
- Оркестрация многоконтейнерных приложений
- Определение сервисов, сетей, volumes

docker-compose.yml
```
version: '3.8'

services:
  web:
    build: .
    ports:
      - "5000:80"
    depends_on:
      - db
    environment:
      - DB_CONNECTION_STRING=Server=db;Database=myapp;User=sa;Password=Your_password123;
  
  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - SA_PASSWORD=Your_password123
      - ACCEPT_EULA=Y
    ports:
      - "1433:1433"
    volumes:
      - sql_data:/var/opt/mssql

volumes:
  sql_data:
```
**Работа с Docker**

**Основные команды**

```
Сборка образа
docker build -t myapp .

Запуск контейнера
docker run -p 5000:80 myapp

Просмотр запущенных контейнеров
docker ps

Остановка контейнера
docker stop <container_id>
```

 **3 DBeaver - работа с БД**

- Универсальный SQL-клиент
- Поддержка SQL Server, PostgreSQL, MySQL и др.
- Визуальный конструктор запросов

[Ссылка на материалы](https://habr.com/ru/articles/738118/)
[Дока](https://dbeaver.io/docs/)
**4 Конфигурация приложения**
**appsettings.json и Environment Variables**

```
// appsettings.json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=myapp;User=sa;Password=12345;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}

// Чтение конфигурации в коде
var connectionString = Configuration.GetConnectionString("Default");
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
```
 **5 Генерация тестовых данных с Bogus**
[ Статья](https://habr.com/ru/articles/673674/)
[Репозиторий](https://github.com/bchavez/Bogus)
```
var testUsers = new Faker<User>()
	    .RuleFor(u => u.Id, f => f.IndexGlobal)
    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
    .RuleFor(u => u.LastName, f => f.Name.LastName())
    .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
    .Generate(100);
    
public static void Initialize(ApplicationDbContext context)
{
    context.Database.EnsureCreated();
    
    if (!context.Users.Any())
    {
        var users = new Faker<User>()
            // правила генерации
            .Generate(50);
            
        context.Users.AddRange(users);
        context.SaveChanges();
    }
}
```
**6 OpenAPI/Swagger документация**
[Swagger](https://habr.com/ru/companies/simbirsoft/articles/707108/)
[Документация](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-8.0)
```

// Startup.cs
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// В Configure
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public ActionResult<Product> GetById(int id)
{
    var product = _repository.GetById(id);
    
    if (product == null)
        return NotFound();
        
    return Ok(product);
}
```

**7 Глобальная обработка ошибок**
[Статья](https://habr.com/ru/companies/otus/articles/543390/)
```
// Custom Exception Middleware
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error"
        }.ToString());
    }
}

app.UseMiddleware<ExceptionMiddleware>();
```

Ссылки:

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
Цель - научиться писать простые веб-сервисы с использованием CA, DDD, SOLID, CQRS подходов
- добавить слои Инфраструктура, Представление
- реализовать логику через паттерн CQRS, DDD

Мы создали клиент серверное приложение. Теперь приведем его к целевому виду согласно Макетам и ТЗ. 

---
## Упражения
Продолжаем реализовывать Систему Электронного тестированяи. Инфраструктуры и представления
В рамках DDD подхода добавим слой Инфрасткруктуры и Представления.
### 1
Обогатите слой инфраструктуры.
Реализовать интерфейсы, которые необходимы для работы приложения 
- IDateTimeProvider
- IEmailService
Устранить циклические зависимости в legacy Application -> Infrastructure 
- Вынести DependencyInjection на уровень слоя представления, чтобы слой Приложения не зависел от инфраструктурного слоя
### 2
Обогатите слой инфраструктуры. Добавьте ORM библиотеку Entity Framework Core (Npgsql.EntityFrameworkCore.PostgreSQL) для работы с базой данных PostgreSQL. 
- реализуйте логику работы с базой данных через DbContext, подключите Базу данных к приложению с использованием Connection String
- настройте отображение сущностей в таблицы через создание IEntityTypeConfiguration

3
Реализуйте репозитории для каждой сущности
- создайте абстрактный класс Repository<> для работы с общими методами
- реализуйте репозитории и их методы для каждой сущности

4
Добавьте в код приложения инфраструктурную логику необходимую для полноценной работы приложения:
- Реализуйте фабрику по созданию соединений для работы с БД IDbConnectionFactory
- Реализуйте публикацию событий после сохранения данных на уровне БД в классе ApplicationDbContext
- Добавьте зависимости в DI по всем созданным классам на предыдущих шагах


5
Для кейса, когда пользователь может отправить несколько запросов на создание ответов по тестированию, реализуйте обработку Race Condition. Это позволит избежать перезаписи неверными данными
- создайте ConcurrencyException, который из себя будет представлять исключение уровня приложения и выбрасываться в случае появления DbUpdateConcurrencyException в методе сохранения UnitOfWork
- доработайте обработчик команды создания ответов с учетом пробрасываемого исключения

6
Создайте проект для слоя представления
- создайте проект ElectronicTestSystem.WebApi и добавьте необходимые файлы: Program, docker, appsettings.json,
- зарегистрируйте DI для слоев Приложения и инфраструктуры

7
Добавьте в слой Представления контроллеры и методы в соответствии с ТЗ. Так как аутентификация и авторизация пока отсутствуют для идентификаторов пользователя передавайте пока что default
- Тесты
- Тестирования для преподавателя
- Тестирования и Ответы на тестирования для учеников

8
Реализуйте деплой приложения через докер контейнер: webapi и db (postgres)
- создайте docker-compose с учетом подключения БД и запуска WebApi
- измените DockerFile 
- доработайте конфигурацию запуска, чтобы можно было запускать деплой через IDE. [ссылка](https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-9.0)
- попробуйте запустить текущее приложение в докер контейнере. Оно должно запуститься без ошибок. Для этого добавьте необходимые зависимости в DI

9
Необходимо подготовить БД для эксплуатации приложением
- добавьте миграции схемы для тестового окружения. Инструменты EFCore.  dotnet ef migrations add Initial --project ElectronicTestSystem.Infrastructure --startup-project ElectronicTestSystem.WebApi --output-dir Migrations
- Исправьте проблему невозможности создания схемы на основе сущностей добавив приватный конструктор в сущности домена
```
   "C:\Program Files\dotnet\dotnet.exe" ef migrations add --project ElectronicTestSystem.Infrastructure\ElectronicTestSystem.Infrastructure.csproj --startup-project ElectronicTestSystem.WebApi\ElectronicTestSystem.WebApi.csproj --context ElectronicTestSystem.Infrastructure.ApplicationDbContext --configuration Debug --verbose Add_IdentityIdToUser --output-dir Migrations
```
- добавьте инициализацию БД тестовыми данными. Добавьте расширение в слой Представление, которое вызывается в дев окружении и наполняет БД тестовыми данными

10
В приложении возникают ошибки необходимо их отловить. Для этого доработайте пайплайн обработки запроса так, чтобы в нем присутствовала глобальная обработка ошибок. Для работы некоторых запросов необходим идентификатор пользователя. Создайте фейковые данные и захордкодьте идентификатор пользователя для тестирования там где это необходимо 
- ошибка роутинга
- ошибка создания Paged опций
- ошибка отправки команд через ISender
- получение пагинированных ответов с условием того, что добавляется фильтр. некорректный ответ в количестве данных, объеме данных


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
