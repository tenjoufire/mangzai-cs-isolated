using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Azure.AI.OpenAI;
using Azure.Identity;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddAzureClients(builder =>
        {
            builder.AddClient<AzureOpenAIClient, AzureOpenAIClientOptions>((options) =>
            {
                var endpoint = new Uri(Environment.GetEnvironmentVariable("OPENAI_ENDPOINT"))
                    ?? throw new InvalidOperationException("openai endpoint is not set");
                var credential = new DefaultAzureCredential();
                return new AzureOpenAIClient(endpoint, credential, options);
            });
        });
    })
    .Build();


host.Run();
