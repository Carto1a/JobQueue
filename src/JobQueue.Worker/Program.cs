using JobQueue.Application;
using JobQueue.Application.Abstractions;
using JobQueue.Application.Commands;
using JobQueue.Infrastructure;
using JobQueue.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.InjectInfrastructure(builder.Configuration);
builder.Services.AddScoped<IProcessJobHandler, ProcessJobHandler>();
builder.Services.AddScoped<JobProcessorResolver>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
