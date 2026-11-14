using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Security;
using RaceDay.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RaceDay")
        ?? "Server=(localdb)\\MSSQLLocalDB;Database=RaceDayDb;Trusted_Connection=True;TrustServerCertificate=True;",
        sql => sql.EnableRetryOnFailure()));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Configuration.GetValue<bool>("Database:InitializeOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RaceDayDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var retries = app.Configuration.GetValue("Database:StartupRetries", 10);
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            if (db.Database.GetMigrations().Any()) await db.Database.MigrateAsync();
            else await db.Database.EnsureCreatedAsync();
            break;
        }
        catch (Exception ex) when (attempt < retries)
        {
            logger.LogWarning(ex, "Database initialization attempt {Attempt} failed.", attempt);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempt * 2, 15)));
        }
    }
}

app.UseSession();
app.MapControllers();

app.Run();

public partial class Program { }
