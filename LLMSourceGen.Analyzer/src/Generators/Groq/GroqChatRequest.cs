namespace LLMSourceGen.Analyzer.Generators.Groq;

internal sealed record GroqChatRequest(
    string Model,
    GroqMessage[] Messages
);
