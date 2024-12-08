namespace LLMSourceGen.Generators.Groq;

internal sealed record GroqChatResponse(
    GroqChatResponse.Choice[] Messages
)
{
    internal sealed record Choice(GroqMessage Message);
}
