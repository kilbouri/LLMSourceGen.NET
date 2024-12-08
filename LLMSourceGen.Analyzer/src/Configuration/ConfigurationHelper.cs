namespace LLMSourceGen.Configuration;

internal static class ConfigurationHelper
{
    private const string VARIABLE_PREFIX = "LLMSourceGen";

    /// <summary>
    /// Fetches the corresponding configuration variable. If the variable is
    /// not set, returns null.
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <returns>The corresponding configuration variable, or null if unset.</returns>
    public static string? GetVariable(string name)
    {
        string environmentVariable = Environment.GetEnvironmentVariable($"{VARIABLE_PREFIX}__{name}");
        return string.IsNullOrWhiteSpace(environmentVariable) ? null : environmentVariable;
    }
}
