using Newtonsoft.Json;

namespace LLMSourceGen.Analyzer.Generators.Groq;

internal sealed class GroqHttpClient : IDisposable
{
    private const string API_ENDPOINT = "https://api.groq.com/openai/v1/chat/completions";

    public string ModelId { get; set; } = "llama3-8b-8192";
    public string? SystemPrompt { get; set; }

    private readonly HttpClient httpClient;
    private bool _disposed = false;

    public GroqHttpClient(string apiKey)
    {
        httpClient = new();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<GroqChatResponse?> SendChatRequestAsync(GroqMessage[] messages, CancellationToken cancellationToken)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new GroqChatRequest()
        {
            Model = ModelId,
            Messages = messages
        }));

        var response = await httpClient.PostAsync(API_ENDPOINT, content, cancellationToken);

        if (!response.IsSuccessStatusCode) return null;

        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GroqChatResponse>(responseBody);
    }

    #region IDisposable
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            httpClient.Dispose();
        }

        _disposed = true;
    }
    #endregion
}
