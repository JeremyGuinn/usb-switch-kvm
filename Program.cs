using Development;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<MonitorController>();
        services.AddSingleton<UsbWatcher>();
        services.Configure<UsbWatcherOptions>(hostContext.Configuration.GetSection("UsbWatcher"));
    })
    .Build();

host.Run();
