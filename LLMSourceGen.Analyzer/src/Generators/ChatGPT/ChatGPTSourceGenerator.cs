using Microsoft.CodeAnalysis;
using LLMSourceGen.Common;
using LLMSourceGen.Generators.Base;

namespace LLMSourceGen.Generators.ChatGPT;

[Generator]
public class ChatGPTSourceGenerator : LLMSourceGenerator<ChatGPTAttribute>
{
    protected override async Task<LLMGeneratedData> PromptLLMAsync(string methodSignature, string prompt, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return new(
            [
                "using System.Text.Json;"
            ],
            [
                "/// <remarks>This was generated!</remarks>"
            ],
            [
                "Console.WriteLine(\"Hello!\");"
            ]);
    }
}
