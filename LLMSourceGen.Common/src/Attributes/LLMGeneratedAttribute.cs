namespace LLMSourceGen.Common;

/// <summary>
/// Base class for all LLM-based source generation attributes.
/// You should not use this directly, and no code will be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class LLMGeneratedAttribute : Attribute
{
    public string Prompt { get; set; } = null!;
}
