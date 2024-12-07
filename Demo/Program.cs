using LLMSourceGen.Common;

new Test1().SayHello();

partial class Test1
{
    [ChatGPT(Prompt = "Say 'Hello'")]
    public partial void SayHello();
}
