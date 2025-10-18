using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Recollect.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS with security
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Production CORS policy for internet access
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
                "http://localhost:7001",
                "https://localhost:7001",
                "http://127.0.0.1:7001",
                "https://127.0.0.1:7001"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=Recollect;Username=postgres;Password=secret";

builder.Services.AddDbContext<AdventureDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use appropriate CORS policy based on environment
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("Production");
}

// Only redirect to HTTPS in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Serve static files for admin interface
app.UseDefaultFiles();
app.UseStaticFiles();

// Admin interface route - serve index.html directly
app.MapGet("/admin", async context =>
{
    context.Response.Redirect("/admin/index.html");
});

app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AdventureDbContext>();
    context.Database.EnsureCreated();
}

app.Run();