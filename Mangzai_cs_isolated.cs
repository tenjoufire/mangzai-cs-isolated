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
        private readonly AzureOpenAIClient _openAIClient;

        public Mangzai_cs_isolated(ILogger<Mangzai_cs_isolated> logger, AzureOpenAIClient openAIClient)
        {
            _logger = logger;
            _openAIClient = openAIClient; 
        }

        /// <summary>
        /// Azure Functions によって呼び出されるメインメソッド
        /// リクエスト本文をJSONで受け取り、その中にあるtextプロパティ情報を使ってOpenAI Chat APIを呼び出す
        /// </summary>
        /// <param name="req"></param>
        /// <returns>JSON形式のHTTPレスポンス</returns>
        [Function("Mangzai_cs_isolated")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //TODO add custom request 

            //リクエストBodyの抽出
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            //リクエストBodyの中にあるテキストを抽出
            string promptText = data?.text ?? string.Empty;

            //TODO use DI to inject the client
            //AOAI クライアントの作成
            /*AzureOpenAIClient client = new(
                new Uri(Environment.GetEnvironmentVariable("OPENAI_ENDPOINT")),
                new DefaultAzureCredential()
            );*/

            //チャットクライアントの作成とAPI呼び出し
            ChatClient chatClient = _openAIClient.GetChatClient(Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT_NAME"));
            ChatCompletion completion = await chatClient.CompleteChatAsync(
                [
                    new SystemChatMessage(Prompts.SystemMessage),
                    new UserChatMessage(promptText)

                ]);
            
            //AOAIからの応答をJSONで返す
            var response = new { text = completion.Content[0].Text };
            string jsonResponse = JsonConvert.SerializeObject(response);
            return new OkObjectResult(jsonResponse);
        }
    }
}
