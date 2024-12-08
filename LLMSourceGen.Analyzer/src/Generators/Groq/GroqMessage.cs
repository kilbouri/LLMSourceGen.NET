namespace LLMSourceGen.Generators.Groq;

internal sealed record GroqMessage(
    string Role,
    string Content
);
