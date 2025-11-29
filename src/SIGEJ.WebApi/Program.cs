using SIGEJ.WebApi;
using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole(); // opcional, se quiser log no console
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddDataAccessLayer();

builder.Services.AddControllers();

var app = builder.Build();

await app.ApplyMigrationsAndSeedAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUI();
    app.UseScalar();
}

app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();