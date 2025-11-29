using SIGEJ.WebApi;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddDataAccessLayer();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Apply migrations and seed for testing
    await app.ApplyMigrationsAndSeedAsync();
    app.UseSwaggerWithUI();
    app.UseScalar();
    
}
else
{
    // Apply migrations only
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();