namespace LLMSourceGen.Generators.Groq;

internal sealed record GroqJsonResponse(
    string[] Usings,
    string[] Documentation,
    string[] Body
);
