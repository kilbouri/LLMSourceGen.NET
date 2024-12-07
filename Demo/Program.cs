using LLMSourceGen.Common;

namespace Demo;

partial class Program
{
    static void Main()
    {
        Program p = new();

        p.SayHello();
        p.FizzBuzz();
        p.SayGoodbye();
    }

    [ChatGPT(Prompt = "Say 'Hello!'")]
    private partial void SayHello();

    [ChatGPT(Prompt = "Print FizzBuzz from 1 to 15")]
    private partial void FizzBuzz();

    [ChatGPT(Prompt = "Say 'Goodbye!'")]
    private partial void SayGoodbye();
}
