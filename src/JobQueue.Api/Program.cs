using JobQueue.Api;
using JobQueue.Application;
using JobQueue.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.InjectInfrastructure(builder.Configuration);
builder.Services.InjectUseCases();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.InitializeDb();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
