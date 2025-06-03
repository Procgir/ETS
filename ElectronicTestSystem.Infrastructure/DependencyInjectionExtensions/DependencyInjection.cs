using Asp.Versioning;
using ElectronicTestSystem.Application.Abstractions;
using ElectronicTestSystem.Application.Abstractions.Authentication;
using ElectronicTestSystem.Application.Abstractions.Clock;
using ElectronicTestSystem.Application.Abstractions.Data;
using ElectronicTestSystem.Application.Caching;
using ElectronicTestSystem.Domain.Abstractions;
using ElectronicTestSystem.Domain.Groups;
using ElectronicTestSystem.Domain.Testings;
using ElectronicTestSystem.Domain.Testings.Calculators;
using ElectronicTestSystem.Domain.Tests;
using ElectronicTestSystem.Domain.Users;
using ElectronicTestSystem.Infrastructure.Authentication;
using ElectronicTestSystem.Infrastructure.Authorization;
using ElectronicTestSystem.Infrastructure.Caching;
using ElectronicTestSystem.Infrastructure.Clock;
using ElectronicTestSystem.Infrastructure.Data;
using ElectronicTestSystem.Infrastructure.Email;
using ElectronicTestSystem.Infrastructure.Groups.Repositories;
using ElectronicTestSystem.Infrastructure.Outbox;
using ElectronicTestSystem.Infrastructure.Testings.Repositories;
using ElectronicTestSystem.Infrastructure.Tests.Repositories;
using ElectronicTestSystem.Infrastructure.Users.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using AuthenticationOptions = ElectronicTestSystem.Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = ElectronicTestSystem.Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = ElectronicTestSystem.Application.Abstractions.Authentication.IAuthenticationService;

namespace ElectronicTestSystem.Infrastructure.DependencyInjectionExtensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IEmailService, EmailService>();

        services.AddScoped<IMarkCalculator, MarkCalculator>();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<ITestingUserAnswerService, TestingUserAnswerService>();
        services.AddScoped<ITestingService, TestingService>();
        services.AddScoped<IGroupService, GroupService>();
        
        AddAuthentication(services, configuration);
        
        AddAuthorization(services);
        
        AddCaching(services, configuration);
        
        AddHealthChecks(services, configuration);
        
        AddApiVersioning(services);

        AddPersistence(services, configuration);
        
        AddBackgroundJobs(services, configuration);

        return services;
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        
        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddTransient<AdminAuthorizationDelegatingHandler>();
        
        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
            {
                var keyCloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                httpClient.BaseAddress = new Uri(keyCloakOptions.AdminUrl);
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();
        
        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();
    }
    
    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }
    
    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Cache") ??
                                  throw new ArgumentNullException(nameof(configuration));

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddSingleton<ICacheService, CacheService>();
    }

    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Cache")!)
            .AddUrlGroup(new Uri(configuration["KeyCloak:BaseUrl"]!), HttpMethod.Get, "keycloak");
    }
    
    private static void AddApiVersioning(IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<ITestRepository, TestEfCoreRepository>();
        services.AddScoped<ITestingRepository, TestingEfCoreRepository>();
        services.AddScoped<ITestingUserAnswersRepository, TestingUserAnswersEfCoreRepository>();
        services.AddScoped<IGroupRepository, GroupEfCoreRepository>();
        services.AddScoped<IUserRepository, UserEfCoreRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetService<ApplicationDbContext>());

        services.AddScoped<ISqlConnectionFactory>(s => new NpgSqlConnectionFactory(connectionString));
    }
    
    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
    }
}