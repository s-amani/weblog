using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;
using PostService.Domain.Events.Interface;
using PostService.Domain.Repositories;
using PostService.Infrastructure.EventHandling;
using PostService.Infrastructure.EventPublishing;
using PostService.Infrastructure.Persistence;
using PostService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService.Application.Services.PostService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

// Register Kafka Event Publisher
builder.Services.AddSingleton<IEventPublisher>(sp =>
{
    var brokerList = builder.Configuration["Kafka:BrokerList"];
    var topic = builder.Configuration["Kafka:Topic"];
    var logger = sp.GetRequiredService<ILogger<KafkaEventPublisher>>();

    return new KafkaEventPublisher(brokerList, topic, logger);
});

// Register Kafka Consumer as Hosted Service
builder.Services.AddHostedService(sp =>
{
    var dispatcher = sp.GetRequiredService<EventHandlerDispatcher>();

    var brokerList = builder.Configuration["Kafka:BrokerList"];
    var topic = builder.Configuration["Kafka:Topic"];
    var groupId = builder.Configuration["Kafka:GroupId"];
    var logger = sp.GetRequiredService<ILogger<KafkaConsumerBackgroundService>>();

    return new KafkaConsumerBackgroundService(dispatcher, brokerList, topic, groupId, logger);
});

var app = builder.Build();

app.MapControllers();

app.UseMiddleware<ExceptionHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        context.Database.Migrate();

        var seeder = new DatabaseSeeder(context);

        seeder.Seed();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "==> An error occurred while seeding the database.");
    }
}


app.Run();