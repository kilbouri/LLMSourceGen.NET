# .NET LLM Source Generation

Large Language Models (LLMs) are the new 🔥 hotness 🔥 for code generation. Why use StackOverflow when you can
get an LLM to generate the answer for you without having to ever open a browser?

> [!WARNING]  
> In case it was not obvious: this project is satire. Please, do not use it. We are 
> not responsible for any damages caused if you choose to use this in your projects.
>
> **Seriously.**

## Advantages

Using LLMs to generate source code has many advantages, including:

- code is written by cutting-edge LLM technology
- all generated code is documented in clear natural language
- generated source code does not require maintenance
- lower costs, as developers can write new features faster

## Limitations

This is an early prototype/proof-of-sarcastic-concept. As such, despite the many advantages, it
does have some minor limitations:

- can only generate non-generic methods
- does not provide much context to the underlying LLM
- does not ensure safe type name resolution
- may hallucinate buggy or completely incoherent source code

## Demonstration

Demonstration images can be found in the [docs](/docs) folder.

## Setup

### Add the Roslyn Tooling

Add the following to your .csproj:

```xml
<ItemGroup>
    <!-- Provides attributes needed to opt into source generation -->
    <ProjectReference Include="Path\To\LLMSourceGen.Common\LLMSourceGen.Common.csproj" />

    <!-- Provides the Analyzer dependency -->
    <ProjectReference Include="Path\To\LLMSourceGen.Analyzer\LLMSourceGen.Analyzer.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>

<ItemGroup>
    <!-- Enable the source generating analyzer -->
    <Analyzer Include="Path\To\LLMSourceGen.Common\bin\Debug\netstandard2.0\LLMSourceGen.Common.dll"></Analyzer>
</ItemGroup>
```

### Groq Backend

To use the free Groq backend, you must generate an API key. You can do that [on Groq's website](https://console.groq.com/keys).
You should store your API key as an environment variable named `LLMSourceGen__GroqKey`. 

You can also choose a model to use using the `LLMSourceGen__GroqModel`. If unset, the default is `llama3-8b-8192`.

## How It Works

### Source Analysis

A Roslyn source generator is provided. The source generator looks for methods that have marker attributes, such as
```cs
namespace Foo;

public partial class LLMExample
{
    // this method will be source generated by an LLM
    [GroqLLMGenerated(Prompt = "Print 'Hello!'")]
    public partial void SayHello();
}
```

The following information is collected about each marked method:

- the specific attribute that opted the method into LLM generation, which indicates the LLM backend to use
- the method modifiers, return type, name, and parameter types and names
- the containing type(s) and namespace of the method, to construct a generated implementation

We collect this specific information so we can provide a more specific prompt to the LLM, in an effort to
reduce errant code generation.

### LLM Generation

We prompt the LLM backend with a special prompt. The exact prompt may vary (see each implementation for exact prompting).
The prompt generally consists of a system message to introduce the model to its new role and outline the following expectations:

- prompts will include a method signature and a user prompt about what the method should do
- responses should contain no "flavor text", be structured - usually in JSON - and contain
  - required imports
  - method documentation
  - a valid method body

### Source Generation

Finally, the parsed response is used to generate a corresponding source. For example, the `SayHello` example
earlier may generate the following code:

```cs
namespace Foo
{
    public partial class LLMExample
    {
        /// <summary>Prints "Hello!" to the console.</summary>
        public partial void SayHello()
        {
            Console.WriteLine("Hello!");
        }
    } 
}
```
