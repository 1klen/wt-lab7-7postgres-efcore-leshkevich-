using Microsoft.EntityFrameworkCore;
using wt_lab6_minimalapi_leshkevich.Data;
using wt_lab6_minimalapi_leshkevich.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // строка подключения
var databaseUrl = builder.Configuration["DATABASE_URL"];

if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var dbPort = uri.Port > 0 ? uri.Port : 5432;

    connectionString =
        $"Host={uri.Host};" +
        $"Port={dbPort};" +
        $"Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={Uri.UnescapeDataString(userInfo[0])};" +
        $"Password={Uri.UnescapeDataString(userInfo[1])};" +
        "SSL Mode=Disable";
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Не задана строка подключения: укажите DATABASE_URL или ConnectionStrings:DefaultConnection.");
}

builder.Services.AddDbContext<AppDbContext>(options => // Регистрация appDbContext (регистрирует контекст в сервисах приложения)
    options.UseNpgsql(connectionString));

// Регистрируем генератор Swagger
builder.Services.AddEndpointsApiExplorer(); // swag
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Включаем Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // интерфейс
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Minimal API работает!");

app.MapGet("/api/info", () =>
    new { Application = "Web Technologies", Technology = "ASP.NET Core Minimal API", Version = "1.0" });

app.MapGet("/api/cars", async (string? search, AppDbContext db) =>
{
    var query = db.Cars.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(x =>
            x.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            x.Model.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    var cars = await query.ToListAsync();
    return Results.Ok(cars);
});

app.MapGet("/api/cars/{id}", async (int id, AppDbContext db) =>
{
    var car = await db.Cars.FindAsync(id);
    if (car == null) return Results.NotFound();
    return Results.Ok(car);
});

app.MapPost("/api/cars", async (Car car, AppDbContext db) =>
{
    car.Id = 0; // Чтобы PostgreSQL сам сгенерировал ID через serial/identity
    await db.Cars.AddAsync(car);
    await db.SaveChangesAsync();
    return Results.Created($"/api/cars/{car.Id}", car);
});

app.MapPut("/api/cars/{id}", async (int id, Car updatedCar, AppDbContext db) =>
{
    var car = await db.Cars.FindAsync(id);
    if (car == null) return Results.NotFound();

    car.Brand = updatedCar.Brand;
    car.Model = updatedCar.Model;
    car.PricePerDay = updatedCar.PricePerDay;
    car.IsAvailable = updatedCar.IsAvailable;

    await db.SaveChangesAsync();
    return Results.Ok(car);
});

app.MapDelete("/api/cars/{id}", async (int id, AppDbContext db) =>
{
    var car = await db.Cars.FindAsync(id);
    if (car == null) return Results.NotFound();

    db.Cars.Remove(car);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}"); // http://localhost:10000/swagger - вот