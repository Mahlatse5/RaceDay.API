using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. Connect to the Database
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Enable Sessions 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Enable CORS for Part 3 (Allows Frontend to talk to API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000", // Standard React port
            "http://localhost:5173", // Vite/React port
            "http://localhost:4200", // Angular port
            "https://localhost:7123" // Blazor port
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // CRITICAL: Required for Session/Cookie authentication to work!
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware order is very important!
app.UseCors("AllowFrontend"); // Must be placed before Session and Authorization
app.UseSession();
app.UseAuthorization();

app.MapControllers();

app.Run();