using System.Security.Cryptography.X509Certificates;
using Asp.Versioning.ApiExplorer;
using ElectronicTestSystem.Application.DependencyInjectionExtensions;
using ElectronicTestSystem.Infrastructure.DependencyInjectionExtensions;
using ElectronicTestSystem.WebApi.Extensions;
using ElectronicTestSystem.WebApi.Healthchecks;
using ElectronicTestSystem.WebApi.OpenApi;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//TODO: удалить. отладка
// DebugContainer(builder);

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (ApiVersionDescription description in app.DescribeApiVersions())
        {
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
    
    app.ApplyMigrations();
    app.SeedData();
//}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

void DebugContainer(WebApplicationBuilder webApplicationBuilder)
{
    string workingDirectory = Environment.CurrentDirectory;
    Console.WriteLine($"Working Directory: {workingDirectory}");
    
    Console.WriteLine("Configuration values:");
    foreach (var config in webApplicationBuilder.Configuration.AsEnumerable())
    {
        Console.WriteLine($"{config.Key} = {config.Value}");
    }

    var certPath = webApplicationBuilder.Configuration["Kestrel:Certificates:Default:Path"];
    var certPassword = webApplicationBuilder.Configuration["Kestrel:Certificates:Default:Password"];

    if (!File.Exists(certPath))
    {
        throw new FileNotFoundException($"Certificate not found at {certPath}");
    }

    var cert = new X509Certificate2(certPath, certPassword);
    if (!cert.HasPrivateKey)
    {
        throw new InvalidOperationException("Certificate has no private key");
    }
}

public partial class Program;