using System.Text.RegularExpressions;

namespace Foo
{
    namespace Bar
    {
        partial class Baz
        {
            partial class Qux
            {
                [GeneratedRegex("a")]
                private static partial Regex GetRegex();
            }
        }
    }
}

namespace Foo.Bar
{
    partial class Baz
    {
        public partial T? Get<T>();
        public partial T? Get<T>() { return default; }
    }


}
