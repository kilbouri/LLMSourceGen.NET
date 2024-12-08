using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using LLMSourceGen.Common.Attributes;
using LLMSourceGen.Analyzer.Exceptions;
using Newtonsoft.Json;

namespace LLMSourceGen.Analyzer.Generators.Base;

public abstract partial class LLMSourceGenerator<TAttribute> : IIncrementalGenerator where TAttribute : LLMGeneratedAttribute
{
    protected sealed record LLMGeneratedData(string[] Usings, string[] Documentation, string[] Body);

    private static readonly string GeneratedCodeMarker = $"GeneratedCodeAttribute(\"{typeof(LLMSourceGenerator<TAttribute>).Assembly.GetName().Name}\", \"{typeof(LLMSourceGenerator<TAttribute>).Assembly.GetName().Version}\")";
    private static readonly string[] Headers = ["// <auto-generated />", "#nullable enable"];

    /// <summary>
    /// Prompts the underlying LLM and returns the parsed response.
    /// </summary>
    /// <param name="boundAttribute">The attribute that triggered the method to be LLM-generated</param>
    /// <param name="methodSignature">The signature of the method being LLM-generated</param>
    /// <param name="prompt">The user-supplied prompt</param>
    /// <returns>The parsed LLM response</returns>
    protected abstract Task<LLMGeneratedData> PromptLLMAsync(string methodSignature, string prompt, CancellationToken cancellationToken);

    private async Task EmitGeneratedCodeAsync(IndentedTextWriter writer, ImmutableArray<LLMMethod?> methods, CancellationToken cancellationToken = default)
    {
        // Write headers followed by an empty line
        foreach (string header in Headers)
        {
            writer.WriteLine(header);
        }
        writer.WriteLine();

        foreach (LLMMethod? method in methods)
        {
            if (method is null) continue;

            LLMGeneratedData llmResponse = await PromptLLMAsync(method.ToDisplayString() + ";", method.UserPrompt, cancellationToken);
            LLMDeclaringType? parent = method.DeclaringType;

            // output the namespace if there is one
            if (!string.IsNullOrWhiteSpace(parent?.Namespace))
            {
                writer.WriteLine($"namespace {parent!.Namespace}");
                writer.WriteLine($"{{");
                writer.Indent++;
            }

            // Output the LLM's requested usings
            foreach (string import in llmResponse.Usings)
            {
                writer.WriteLine(import);
            }
            writer.WriteLine();

            // Invert the parent tree, because we have to write the top-most parent first and the containing type last
            Stack<string> parentClasses = new();
            while (parent is not null)
            {
                parentClasses.Push($"partial {parent.Keyword} {parent.Name}");
                parent = parent.Parent;
            }

            while (parentClasses.Count != 0)
            {
                writer.WriteLine($"{parentClasses.Pop()}");
                writer.WriteLine($"{{");
                writer.Indent++;
            }


            // Write documentation
            foreach (string commentLine in llmResponse.Documentation)
            {
                writer.WriteLine(commentLine);
            }

            // Mark method as generated code
            writer.WriteLine($"[global::System.CodeDom.Compiler.{GeneratedCodeMarker}]");

            // Write the method itself
            writer.WriteLine(method.ToDisplayString());
            writer.WriteLine("{");
            writer.Indent++;
            foreach (string line in llmResponse.Body)
            {
                writer.WriteLine(line);
            }
            writer.Indent--;
            writer.WriteLine("}");

            // Unwind remaining scopes
            while (writer.Indent != 0)
            {
                writer.Indent--;
                writer.WriteLine("}");
            }
        }
    }
}
