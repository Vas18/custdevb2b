using custdev.business.services;
using custdev.db.cosmos.services;
using custdev.domain.configuration;
using custdev.domain.interfaces;
using custdev.domain.interfaces.db;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.Configure<CosmosDbConfiguration>(context.Configuration.GetSection("CosmosDbConfig"));
        services.Configure<OpenAiDefaultConfiguration>(context.Configuration.GetSection("OpenAiDefaultConfiguration"));
        services.Configure<OpenAiApiAccountConfiguration>(context.Configuration.GetSection("OpenAiDefaultConfiguration"));

        services.AddScoped<IAiService, OpenAiSingleService>();
        services.AddScoped<IDbMessageService, DbMessageService>();
        //

    })
    .Build();

host.Run();
