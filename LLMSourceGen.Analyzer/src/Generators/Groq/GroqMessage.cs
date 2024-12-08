namespace LLMSourceGen.Analyzer.Generators.Groq;

internal sealed record GroqMessage(
    string Role,
    string Content
);
