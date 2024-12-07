namespace LLMSourceGen.Common;

/// <summary>
/// Base class for all LLM-based source generation attributes.
/// You should not use this directly.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class LLMAttribute : Attribute
{
    public string Prompt { get; set; } = null!;
}
