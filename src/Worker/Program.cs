using Infrastructure;
using Serilog;
using Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructureForWorker(builder.Configuration);
builder.Services.AddHostedService<DocumentWorker>();

builder.Services.AddSerilog(lc => lc.ReadFrom.Configuration(builder.Configuration));

IHost host = builder.Build();
await host.RunAsync();
