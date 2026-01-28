using JobQueue.Application.Commands;
using JobQueue.Dispatcher;
using JobQueue.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.InjectInfrastructure(builder.Configuration);
builder.Services.AddScoped<DispatchPendingJobsHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
