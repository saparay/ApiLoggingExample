using ApiLoggingExample.Data;
using ApiLoggingExample.Entities;
using ApiLoggingExample.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<LoggingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(
    new SqlLoggerProvider(
        builder.Services.BuildServiceProvider(),
        builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()
    ));
builder.Services.AddSingleton<ILogQueue, LogQueue>();

builder.Services.AddControllers();

builder.Services.AddScoped<RequestLogContext>();

builder.Services.AddHostedService<LogWorker>();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<FinalRequestLoggingMiddleware>();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
