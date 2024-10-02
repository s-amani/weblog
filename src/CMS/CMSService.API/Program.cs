var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddLogging();

var app = builder.Build();

app.MapControllers();

// app.UseMiddleware<ExceptionHandlerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // var context = services.GetRequiredService<AppDbContext>();
        //
        // context.Database.Migrate();
        //
        // var seeder = new DatabaseSeeder(context);
        //
        // seeder.Seed();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "==> An error occurred while seeding the database.");
    }
}


app.Run();