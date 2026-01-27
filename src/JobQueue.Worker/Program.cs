using JobQueue.Application;
using JobQueue.Infrastructure;
using JobQueue.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.InjectInfrastructure(builder.Configuration);
builder.Services.AddScoped<ProcessJobHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
