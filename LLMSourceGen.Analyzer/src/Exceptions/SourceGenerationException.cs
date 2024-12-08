namespace LLMSourceGen.Analyzer.Exceptions;

internal class SourceGenerationException(string message, Exception? innerException = null)
    : Exception(message, innerException);
