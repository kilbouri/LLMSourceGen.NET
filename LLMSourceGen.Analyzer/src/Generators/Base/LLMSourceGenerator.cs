using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LLMSourceGen.Common;

namespace LLMSourceGen.Generators.Base;

public abstract partial class LLMSourceGenerator<TAttribute> : IIncrementalGenerator where TAttribute : LLMGeneratedAttribute
{
    internal sealed record LLMDeclaringType(string Keyword, string Namespace, string Name)
    {
        public LLMDeclaringType? Parent;
    }

    internal sealed record LLMMethod(
        LLMDeclaringType DeclaringType,
        string Modifiers,
        string ReturnType,
        string MethodName,
        List<LLMMethod.LLMMethodParameter> Arguments,
        string UserPrompt
    )
    {
        internal sealed record LLMMethodParameter(string Type, string Name);

        public string ToDisplayString() => $"{Modifiers} {ReturnType} {MethodName}({string.Join(", ", Arguments.Select(x => $"{x.Type} {x.Name}"))})";
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodsToGenerate = context
                                    .SyntaxProvider
                                    .ForAttributeWithMetadataName(
                                        fullyQualifiedMetadataName: typeof(TAttribute).FullName!,
                                        predicate: static (_, _) => true,
                                        transform: static (ctx, _) => GetMethodInfo(ctx.SemanticModel, ctx.TargetNode, ctx.Attributes[0])
                                    )
                                    .Where(static m => m is not null)
                                    .Collect();

        context.RegisterSourceOutput(methodsToGenerate, async (context, results) =>
        {
            using StringWriter rawWriter = new();
            using IndentedTextWriter writer = new(rawWriter);

            await EmitGeneratedCodeAsync(writer, results, context.CancellationToken);

            context.AddSource($"LLMSourceGenerator.{typeof(TAttribute).FullName}.g.cs", rawWriter.ToString());
        });
    }

    private static LLMMethod? GetMethodInfo(SemanticModel semanticModel, SyntaxNode methodDeclarationSyntax, AttributeData boundAttribute)
    {
        if (semanticModel.GetDeclaredSymbol(methodDeclarationSyntax) is not IMethodSymbol methodSymbol) return null;

        LLMDeclaringType? declaringType = ResolveDeclaringType(semanticModel, methodDeclarationSyntax);
        if (declaringType is null) return null;

        string? userPrompt = GetUserPrompt(boundAttribute);
        if (userPrompt is null) return null;

        if (methodDeclarationSyntax is not MemberDeclarationSyntax memberDeclarationSyntax) return null;

        string returnType = methodSymbol.ReturnsVoid
                                ? "void"
                                : methodSymbol.ReturnType.Name + (methodSymbol.ReturnNullableAnnotation == NullableAnnotation.Annotated ? "?" : "");

        return new(
            declaringType,
            memberDeclarationSyntax.Modifiers.ToString(),
            returnType,
            methodSymbol.Name,
            methodSymbol.Parameters.Select(param => new LLMMethod.LLMMethodParameter(param.Type.Name, param.Name)).ToList(),
            userPrompt
        );
    }

    private static string? GetDeclaringNamespace(IMethodSymbol methodSymbol) => methodSymbol
        .ContainingType?
        .ContainingNamespace?
        .ToDisplayString(
            SymbolDisplayFormat
                .FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
        );

    private static LLMDeclaringType? ResolveDeclaringType(SemanticModel semanticModel, SyntaxNode methodDeclarationSyntax)
    {
        if (methodDeclarationSyntax.Parent is not TypeDeclarationSyntax typeDec) return null;
        if (semanticModel.GetDeclaredSymbol(methodDeclarationSyntax) is not IMethodSymbol methodSymbol) return null;

        string? ns = GetDeclaringNamespace(methodSymbol);
        LLMDeclaringType result = ToHoldingType(typeDec, ns);

        // Traverse the type declaration tree all the way to the top
        LLMDeclaringType current = result;
        var parent = typeDec.Parent as TypeDeclarationSyntax;

        while (parent is not null && IsAllowedKind(parent.Kind()))
        {
            current.Parent = ToHoldingType(parent, ns);

            current = current.Parent;
            parent = parent.Parent as TypeDeclarationSyntax;
        }

        return result;

        static LLMDeclaringType ToHoldingType(TypeDeclarationSyntax syntax, string? ns) => new(
            syntax is RecordDeclarationSyntax rds ? $"{syntax.Keyword.ValueText} {rds.ClassOrStructKeyword}" : syntax.Keyword.ValueText,
            ns ?? string.Empty,
            $"{syntax.Identifier}{syntax.TypeParameterList}"
        );

        static bool IsAllowedKind(SyntaxKind kind) => kind is
            SyntaxKind.ClassDeclaration or
            SyntaxKind.StructDeclaration or
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration or
            SyntaxKind.InterfaceDeclaration;
    }

    private static string? GetUserPrompt(AttributeData attribute) => attribute
        .NamedArguments
        .Where(pair => pair.Key == nameof(LLMGeneratedAttribute.Prompt))
        .Select(pair => pair.Value.Value as string)
        .FirstOrDefault();
}
