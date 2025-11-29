using SIGEJ.WebApi.Data;
using SIGEJ.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole(); // opcional, se quiser log no console
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddDataAccessLayer();
builder.Services.AddApplicationLayer();

builder.Services.AddControllers();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

using (var scope = app.Services.CreateScope())
{
    // Migrate Database 
    var database = scope.ServiceProvider.GetRequiredService<Database>();
    await database.MigrateAsync();
    
    // Seed Database
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
    await seeder.RunAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
await app.RunAsync();