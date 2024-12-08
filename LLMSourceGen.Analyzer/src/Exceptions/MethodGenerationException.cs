namespace LLMSourceGen.Analyzer.Exceptions;

internal class MethodGenerationException(string methodSignature, string reason, Exception? innerException = null)
    : SourceGenerationException($"Failed to generate method '{methodSignature}' - {reason}", innerException);
