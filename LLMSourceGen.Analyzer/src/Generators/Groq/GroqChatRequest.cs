namespace LLMSourceGen.Analyzer.Generators.Groq;

internal class GroqChatRequest
{
    public string Model { get; init; } = string.Empty;
    public GroqMessage[] Messages { get; init; } = [];
}
