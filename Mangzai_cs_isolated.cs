using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.AI.OpenAI;
using Azure.Identity;
using Newtonsoft.Json;
using OpenAI.Chat;

namespace Company.Function
{
    public class Mangzai_cs_isolated
    {
        private readonly ILogger<Mangzai_cs_isolated> _logger;

        public Mangzai_cs_isolated(ILogger<Mangzai_cs_isolated> logger)
        {
            _logger = logger;
        }

        [Function("Mangzai_cs_isolated")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string promptText = data?.text ?? string.Empty;

            AzureOpenAIClient client = new(
                new Uri(Environment.GetEnvironmentVariable("OPENAI_ENDPOINT")),
                new DefaultAzureCredential()
            );
            ChatClient chatClient = client.GetChatClient(Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME"));
            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage(Prompts.SystemMessage),
                    new UserChatMessage(promptText)

                ]);

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
