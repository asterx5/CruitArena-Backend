using CruitArena.Hubs;
using CruitArena.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services as singletons (in-memory state)
builder.Services.AddSingleton<RoomManager>();
builder.Services.AddSingleton<GameManager>();

// Add SignalR with JSON options for polymorphic GameAction deserialization
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// CORS for development (allow Kotlin client to connect)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true); // Allow all origins for dev
    });
});

var app = builder.Build();

app.UseCors();

app.MapHub<GameHub>("/gamehub");

// Simple health check endpoint
//app.MapGet("/", () => "CruitArena Server is running");

app.Run();
