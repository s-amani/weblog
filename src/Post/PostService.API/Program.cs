using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;
using PostService.Domain.Events;
using PostService.Domain.Events.Interface;
using PostService.Domain.Interfaces;
using PostService.Domain.Repositories;
using PostService.Infrastructure.EventHandling;
using PostService.Infrastructure.EventPublishing;
using PostService.Infrastructure.Persistence;
using PostService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddLogging();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IPostService, PostService.Application.Services.PostService>();
builder.Services.AddScoped<ICategoryService, PostService.Application.Services.CategoryService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

// Register Kafka Event Publisher
builder.Services.AddSingleton<IEventPublisher>(sp =>
{
    var brokerList = builder.Configuration["Kafka:BrokerList"];
    var topics = builder.Configuration.GetSection("Kafka:Topics").Get<List<string>>();

    var logger = sp.GetRequiredService<ILogger<KafkaEventPublisher>>();

    return new KafkaEventPublisher(brokerList, topics, logger);
});

builder.Services.AddScoped<IEventHandler<CategoryDeletedEvent>, CategoryDeletedEventHandler>();

// Register event handler dispatcher
builder.Services.AddSingleton<EventHandlerDispatcher>();

// Register Kafka Consumer as Hosted Service
builder.Services.AddHostedService(sp =>
{
    var dispatcher = sp.GetRequiredService<EventHandlerDispatcher>();

    var brokerList = builder.Configuration["Kafka:BrokerList"];
    var groupId = builder.Configuration["Kafka:GroupId"];
    var topics = builder.Configuration.GetSection("Kafka:Topics").Get<List<string>>();

    var logger = sp.GetRequiredService<ILogger<KafkaConsumerBackgroundService>>();
    var configuration = sp.GetRequiredService<IConfiguration>();

    return new KafkaConsumerBackgroundService(dispatcher, brokerList, topics, groupId, logger, configuration);
});

var app = builder.Build();

app.MapControllers();

//app.UseMiddleware<ExceptionHandlerMiddleware>();

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