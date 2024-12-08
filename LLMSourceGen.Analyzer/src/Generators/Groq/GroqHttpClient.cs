using Newtonsoft.Json;

namespace LLMSourceGen.Generators.Groq;

internal sealed class GroqHttpClient(string ApiKey) : IDisposable
{
    private const string API_ENDPOINT = "https://api.groq.com/openai/v1/chat/completions";

    public string ModelId { get; set; } = "llama3-8b-8192";
    public string? SystemPrompt { get; set; }

    private readonly HttpClient httpClient = new();
    private bool _disposed = false;

    public async Task<GroqChatResponse?> SendChatRequestAsync(GroqChatRequest request, CancellationToken cancellationToken)
    {
        StringContent requestContent = new(":");
        requestContent.Headers.Add("Authorization", $"Bearer {ApiKey}");

        var response = await httpClient.PostAsync(API_ENDPOINT, new StringContent(JsonConvert.SerializeObject(request)), cancellationToken);
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
