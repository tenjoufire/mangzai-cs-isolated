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

        /// <summary>
        /// Azure Functions によって呼び出されるメインメソッド
        /// リクエスト本文をJSONで受け取り、その中にあるtextプロパティ情報を使ってOpenAI Chat APIを呼び出す
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Function("Mangzai_cs_isolated")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //リクエストBodyの抽出
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            //リクエストBodyの中にあるテキストを抽出
            string promptText = data?.text ?? string.Empty;

            // Create an OpenAI client
            AzureOpenAIClient client = new(
                new Uri(Environment.GetEnvironmentVariable("OPENAI_ENDPOINT")),
                new DefaultAzureCredential()
            );

            // Get the chat client
            ChatClient chatClient = client.GetChatClient(Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME"));
            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage(Prompts.SystemMessage),
                    new UserChatMessage(promptText)

                ]);
            
            // Return the response as JSON
            var response = new { text = completion.Content[0].Text };
            string jsonResponse = JsonConvert.SerializeObject(response);
            return new OkObjectResult(jsonResponse);
        }
    }
}
