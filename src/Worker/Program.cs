using Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DocumentWorker>();

IHost host = builder.Build();
await host.RunAsync();
