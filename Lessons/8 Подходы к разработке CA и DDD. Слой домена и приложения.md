## Цели и задачи
Сформировать представление о [Чистой архитектуре ](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)(CleanArchitecture), [Предметно-ориентированном подходе ](https://ru.wikipedia.org/wiki/%D0%9F%D1%80%D0%B5%D0%B4%D0%BC%D0%B5%D1%82%D0%BD%D0%BE-%D0%BE%D1%80%D0%B8%D0%B5%D0%BD%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%BD%D0%BE%D0%B5_%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5)([DDD](https://www.youtube.com/watch?v=6_BhdXLxiic)) и освоить разработку с помощью этих подходов на примере Системы Электронного тестирования
- реализовать слой Домена в подходе CA + CQRS + DDD
- реализовать слой Приложения CA + CQRS + DDD
- реализовать простое получение данных с помощью библиотеки Dapper
- реализовать CCC (Cross-Cutting Concern) Logging. Логгирование команд
- реализовать CCC Validation. Валидация команд

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

#### Слой Домена
Задача: **описывает бизнес-сущности и правила**

- **Entity** — имеет идентичность
- **ValueObject** — immutable, без идентичности
- **Aggregate Root** — точка входа к связанным сущностям
- **Domain Events** — отражают важные бизнес-события
- **Repositories (абстракции)** — для доступа к агрегатам

#### Слой приложения
Задача: **координирует вызовы между слоями, содержит use cases**

- **Сервисы (Application Services)** — реализация бизнес-операций
- **Команды/Запросы** — структура CQRS
- **Интерфейсы для Domain и Infrastructure**

**Плюсы:**
- Независимость от внешних решений.
- Легкая замена компонентов (например, переход с EF Core на Dapper).
- Удобное тестирование.

**Минусы:**
- Избыточность для маленьких проектов.
- Большое количество boilerplate-кода.

**CQRS (Command Query Responsibility Segregation)**

CQRS — это архитектурный паттерн, который разделяет операции чтения (Query) и записи (Command) данных в системе.

**Основные идеи:**

- **Команды (Commands)** изменяют состояние системы, но не возвращают данные (например, `CreateUser`, `UpdateOrder`).
- **Запросы (Queries)** возвращают данные, но не изменяют состояние (например, `GetUserById`, `ListOrders`).
- Разные модели для чтения и записи (может быть разная БД, схема или даже сервис).

**Плюсы:**
- Масштабируемость (можно оптимизировать чтение отдельно от записи).
- Гибкость (разные хранилища для разных задач).
- Упрощение сложных сценариев (например, Event Sourcing).

**Минусы:**
- Усложнение архитектуры.
- Сложность поддержания согласованности данных.
- Не нужен в простых CRUD-приложениях.

**Пример:**
- **Command-сторона:** обрабатывает `CreateOrderCommand`, сохраняет в БД и публикует событие `OrderCreated`.
- **Query-сторона:** использует оптимизированную denormalized БД для быстрого отображения списка заказов.

Плюсы:

- Чёткое разделение ответственности
- Упрощение кода
- Масштабируемость

**CQRS + Clean Architecture**
Часто CQRS применяют внутри Clean Architecture:

- **Commands и Queries** – это Use Cases в слое Application.
- **Domain** содержит бизнес-правила.
- **Infrastructure** реализует репозитории для CQRS (например, отдельные таблицы для Query-стороны).


Ссылки:
[Robert Martin Blog](https://blog.cleancoder.com/)
[Robert Martin CA](https://blog.cleancoder.com/uncle-bob/2011/11/22/Clean-Architecture.html)
[Onion Part1](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)
[Onion Part2](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-2/)
[Onion Part3](https://jeffreypalermo.com/2008/08/the-onion-architecture-part-3/)
[Onion Part4](https://jeffreypalermo.com/2013/08/onion-architecture-part-4-after-four-years/)
### DDD
**DDD** фокусируется на бизнес-логике и предметной области. Его цель — моделировать поведение системы, отталкиваясь от терминов и логики, понятных бизнесу.

**1. Entity (Сущность)**

- Объект, обладающий уникальной идентичностью (ID).
    
- Поведение важнее структуры.
    
- Сравнение происходит по ID, а не по значению.
    

Пример:
```
public class Customer {
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Customer(Guid id, string name) {
        Id = id;
        Name = name;
    }

    public void Rename(string newName) {
        Name = newName;
    }

    public override bool Equals(object obj) {
        return obj is Customer other && Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}
```

---

**2. Value Object (Объект-значение)**

- Не имеет идентичности.
    
- Неизменяемый (immutable).
    
- Сравнение по значению.
    

Пример:

```
public record Money(decimal Amount, string Currency);
public record Address(string City, string Street, string PostalCode);
```

---

**3. Aggregate (Агрегат)**

- Группа связанных сущностей и объектов-значений.
    
- Имеет Aggregate Root — главный объект, через который осуществляется доступ.
    
- Гарантирует целостность внутри границ агрегата.
    

Пример:

```
public class Order {
    public Guid Id { get; private set; }
    private List<OrderItem> _items = new();

    public IReadOnlyCollection<OrderItem> Items => _items;

    public void AddItem(Guid productId, int quantity) {
        var item = new OrderItem(productId, quantity);
        _items.Add(item);
    }
}

public class OrderItem {
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    public OrderItem(Guid productId, int quantity) {
        ProductId = productId;
        Quantity = quantity;
    }
}
```
---

**4. Aggregate Root (Корень агрегата)**

- Единственная точка входа к агрегату.
- Управляет внутренним состоянием и поведением.
- Внешний код не может напрямую модифицировать дочерние сущности.

---

**5. Domain Event (Событие предметной области)**

- Сигнализирует, что произошло важное бизнес-событие.
- Отделяет реакцию от инициирующей логики.

Пример:
```
public record OrderCreatedEvent(Guid OrderId) : IDomainEvent;

public interface IDomainEvent {}
```

---

**6. Repository (Репозиторий)**

- Абстракция для доступа к агрегатам.
- Изолирует доменную модель от базы данных.
- Работает с агрегатами как с in-memory коллекциями.

Пример:
```
public interface IOrderRepository {
    Task<Order> GetByIdAsync(Guid id);
    Task AddAsync(Order order);
}
```

---

**7. Domain Service (Доменный сервис)**

- Используется, если бизнес-логика не вписывается в одну сущность.
- Содержит операции, оперирующие несколькими агрегатами.
    

Пример:
```
public class TransferService {
    public void TransferMoney(Account from, Account to, Money amount) {
        from.Withdraw(amount);
        to.Deposit(amount);
    }
}
```

---
**8. Factory (Фабрика)**

- Создает агрегаты, соблюдая инварианты.
- Скрывает детали создания.
    
Пример:
```
public static class OrderFactory {
    public static Order CreateNew(List<(Guid productId, int quantity)> items) {
        var order = new Order(Guid.NewGuid());
        foreach (var item in items)
            order.AddItem(item.productId, item.quantity);
        return order;
    }
}
```
---

## 🧠 Что это даёт

- Разделение ответственности: бизнес-логика = в домене, хранение = в репозиториях.
- Высокая тестируемость: легко тестировать доменные правила без базы.
- Гибкость и адаптация к изменениям бизнеса.

[MediatR](https://github.com/jbogard/MediatR) - библиотека для реализации подхода CQRS

### ORM (Object-Relational Mapper)
ORM (Object-Relational Mapping) - это технология, которая связывает базы данных с объектно-ориентированными языками программирования. Она позволяет работать с данными как с объектами в коде, вместо написания SQL-запросов вручную.

Основные принципы ORM:
1. Таблицы в БД отображаются на классы в коде
2. Строки таблиц становятся объектами этих классов
3. Колонки таблиц отображаются на свойства объектов
4. ORM автоматически генерирует SQL-запросы

Преимущества ORM:
- Упрощает работу с БД
- Уменьшает количество кода
- Обеспечивает безопасность от SQL-инъекций
- Автоматизирует многие задачи

Недостатки ORM:
- Может быть менее производительным, чем чистый SQL
- Требует изучения
- Иногда сложно оптимизировать запросы

Популярные ORM для .NET:
- Entity Framework (полнофункциональный ORM)
- Dapper (микро-ORM)
- NHibernate

[Dapper](https://github.com/DapperLib/Dapper) - это быстрый и легкий микро-ORM для .NET, который работает как расширение для IDbConnection. Он позволяет выполнять SQL-запросы и автоматически маппить результаты в объекты. В отличие от полноценных ORM вроде Entity Framework, Dapper не имеет сложных функций вроде трекинга изменений, что делает его очень производительным.

```
using Dapper;
using System.Data.SqlClient;

var connection = new SqlConnection("YourConnectionString");
var users = connection.Query<User>("SELECT * FROM Users WHERE Active = @Active", 
    new { Active = true });

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
}
```

Основные плюсы Dapper:
- Очень высокая скорость работы (близко к чистому ADO.NET)
- Простота использования
- Работает с разными базами данных (SQL Server, PostgreSQL, MySQL и др.)
- Минимальная настройка - просто пишете SQL и получаете объекты

Когда стоит выбирать Dapper:
- Когда критична скорость работы с базой
- Когда нужен полный контроль над SQL-запросами
- В высоконагруженных приложениях (API, микросервисы)
[Туториал](https://dappertutorial.net/)
### CCC (Cross-Cutting Concern)
CCC — сквозная функциональность, которая:
- **Не относится к бизнес-логике**, но нужна везде.
- **Дублируется** в разных слоях приложения.
- **Усложняет код**, если реализована прямо в бизнес-логике.

**Примеры:**
- Логирование
- Валидация
- Кэширование
- Транзакции
- Обработка ошибок
Логирование (Logging)

**Проблема:**
- Логи нужны в каждом методе, но их ручное добавление приводит к **дублированию кода**.

**Решение:**
- **Декораторы**
- **АОП (Aspect-Oriented Programming)**
- **Middleware (в ASP.NET Core)**

Пример MediatR
```
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken ct, RequestHandlerDelegate<TResponse> next)
    {
        Console.WriteLine($"Запрос: {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Ответ: {typeof(TResponse).Name}");
        return response;
    }
}
```
**Валидация (Validation)**

**Проблема:**
- Проверка входных данных повторяется в разных командах/запросах.

**Решение:**
- **FluentValidation**
- **PipelineBehavior в MediatR**

**Пример с MediatR:**
```
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, CancellationToken ct, RequestHandlerDelegate<TResponse> next)
    {
        var errors = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (errors.Any())
            throw new ValidationException(errors);

        return await next();
    }
}
```
**Реализация в MediatR**

MediatR поддерживает **Pipeline Behaviors** — цепочку обработчиков, через которые проходят запросы.

**Как подключить:**
1. Создаем поведение (например, для логирования/валидации).
2. Регистрируем в DI:

```
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

**Порядок выполнения:**  
Запрос → Логирование → Валидация → Бизнес-логика → Ответ.

**Итог**
- **Cross-Cutting Concerns** выносятся в отдельные компоненты.
- **MediatR Pipeline** позволяет применять их глобально.
- Код становится **чище** и **легче поддерживается**.

### Виды архитектур
[Microsoft Blog](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures) о общих архитектурах веб
[Архитектурные принципы](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)
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
- добавить слои Домен, Приложение
- реализовать логику через паттерн CQRS, DDD

Мы создали клиент серверное приложение. Теперь приведем его к целевому виду согласно Макетам и ТЗ. 

---
## Упражения
Возвращаемся в настоящее, где нам необходимо разработать систему Электронного тестирования учеников. 
Необходимо привести наше приложение к виду, который мы получили в результате анализа предметной области.
В рамках DDD подхода доработаем доменный слой и слой приложений
### 1
Добавить в слой домена сущность Теста  и сервисные классы, необходимые для дальнейшей реализации приложения. Также необходимо переделать приложение, чтобы оно обеспечивало обратную совместимость с UI консольных приложений. 
По необходимости добавьте классы/методы, реализующие логику на уровне слоя приложения

### 2
Добавить в слой домена сущность Пользователь и Группа и сервисные классы, необходимые для дальнейшей реализации приложения. Также необходимо переделать приложение, чтобы оно обеспечивало обратную совместимость с UI консольных приложений. 
По необходимости добавьте классы/методы, реализующие логику на уровне слоя приложения

3
Добавить в слой домена и по необходимости слой приложения сущность Тестирование и сервисные классы, необходимые для дальнейшей реализации приложения. Также необходимо переделать приложение, чтобы оно обеспечивало обратную совместимость с UI консольных приложений. 
По необходимости добавьте классы/методы, реализующие логику на уровне слоя приложения

4
Доработать класс Result
Доработать логику на основе класса Result и добавить валидацию и проверки

5
Вы по инерции продолжили реализовывать логику приложения с учетом фасадной структуры интферфейсов. Используем новый подход CQRS паттерн.
Добавить в слой приложения реализацию CQRS для работы с Тестами

6
Добавить в слой приложения реализацию CQRS для работы с Тестированием

7
Добавить в слой приложения реализацию CQRS для работы с  Ответами на тестирование

8
рассмотрим некоторые механизмы, которые можно использовать при работе с CQRS. В случае запросов (Query), обычно нет никакой сложной логики по формированию набора данных, которые мы запрашиваем. В таким случаях можем использовать легковесные библиотеки, которые позволяют обращаться напрямую за данными, избегая кучи прослоек между доменом и приложением. Что значительно экономит время, ресурсы и производительность.
Продемонстрируем на примере получения тестов данный подход. Необходимо переписать запрос GetTestQuery с использованием библиотеки Dapper и системного интерфейса соединений с БД (IDbConnection), в связке с которым библиотека добавляет методы расширения для доступа к данным в БД.

9
Реализовать метод поиска тестов GetTestsQuery по запросу с использованием библиотеки Dapper

10
Добавить CCC Логгирование команд перед их выполнением и после их успешного/неудачного завершения (с учетом возможного выброса исключений) с использованием библиотеки MediatR через добавление PipelineBehavior, ILogging из стандартной библиотеки Microsoft.Extensions.Logging.Abstractions

11
Добавить CCC Валидацию команд перед их выполнением  с использованием библиотеки MediatR через добавление PipelineBehavior и использования библиотеки FluentValidation для построения логики валидации с помощью аттрибутов. Добавить валидацию для AssignTestingCommand

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
