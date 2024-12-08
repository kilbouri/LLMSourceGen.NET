namespace LLMSourceGen.Analyzer.Generators.Groq;

internal sealed class GroqChatResponse
{
    internal sealed class Choice
    {
        internal sealed class ChoiceMessage
        {
            public string Role { get; init; } = string.Empty;
            public string Content { get; init; } = string.Empty;
        }

        public int Index { get; init; } = 0;
        public ChoiceMessage Message { get; init; } = new();
    }

    public string Model { get; init; } = string.Empty;
    public Choice[] Choices { get; init; } = [];
}
