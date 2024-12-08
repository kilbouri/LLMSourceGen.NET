namespace LLMSourceGen.Analyzer.Generators.Groq;

internal sealed class GroqJsonResponse
{
    public string[] Usings { get; init; } = [];
    public string[] Documentation { get; init; } = [];
    public string[] Body { get; init; } = [];
}
