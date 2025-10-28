using Microsoft.EntityFrameworkCore;
using QuizEngineBE.Models;
using QuizEngineBE.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//-------------------- Serilog --------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

//------------------- DbContext --------------------
var connectionString = builder.Configuration.GetConnectionString("QuizDb");
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlServer(connectionString));

//-------------------- Dependency Injection --------------------
builder.Services.AddScoped<DbService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<QuizEngineService>();
builder.Services.AddScoped<QuizService>();

builder.Services.AddSingleton<SecurityService>();


//======================================= COSTRUZIONE APP =======================================



var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

Log.Information("quiz engine attivato");

app.Run();
