namespace DocoptNet.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class GetNodesTests
    {
        [Test]
        public void Should_get_nodes()
        {
            const string USAGE = @"Test host app for T4 Docopt.NET

Usage:
  prog command ARG <myarg> [OPTIONALARG] [-o -s=<arg> --long=ARG --switch]
  prog files FILE...

Options:
 -o --opt     Short switch with a longer name. The longer name is the ""main"" one.
 -s=<arg>     Short option with arg.
 --long=ARG   Long option with arg.
 --switch     Long switch. [default: false]

Explanation:
 This is a test ""usage"".
";

            var actual = new Docopt().GetNodes(USAGE);

            CollectionAssert.AreEqual(
                new Node[]
                {
                    new CommandNode ("command"),
                    new ArgumentNode("ARG",         ValueType.String),
                    new ArgumentNode("<myarg>",     ValueType.String),
                    new ArgumentNode("OPTIONALARG", ValueType.String),
                    new OptionNode  ("opt",         ValueType.Bool),
                    new OptionNode  ("s",           ValueType.String),
                    new OptionNode  ("long",        ValueType.String),
                    new OptionNode  ("switch",      ValueType.Bool),
                    new CommandNode ("files"),
                    new ArgumentNode("FILE",        ValueType.List),
                },
                actual);
        }

        [Test]
        public void Should_return_duplicates_so_caller_knows_which_args_are_after_each_command()
        {
            const string USAGE = @"Test for duplicate commands and arguments.
Usage:
  prog command ARG <myarg> [OPTIONALARG] [-o -s=<arg> --long=ARG --switch]
  prog command ARG <myarg> [OPTIONALARG] [-o -s=<arg> --long=ARG --switch] FILE...
  prog diff-command <myarg> [OPTIONALARG] ARG

Options:
 -o --option  Short & long option flag, the long will be used as it's more descriptive.
 -s=<arg>     Short option with arg.
 --long=ARG   Long option with arg.
 --switch     Long switch.
";

            var actual = new Docopt().GetNodes(USAGE);

            CollectionAssert.AreEqual(
                new Node[]
                {
                    new CommandNode ("command"),
                    new ArgumentNode("ARG",         ValueType.String),
                    new ArgumentNode("<myarg>",     ValueType.String),
                    new ArgumentNode("OPTIONALARG", ValueType.String),
                    new OptionNode  ("option",      ValueType.Bool),
                    new OptionNode  ("s",           ValueType.String),
                    new OptionNode  ("long",        ValueType.String),
                    new OptionNode  ("switch",      ValueType.Bool),
                    new CommandNode ("command"),
                    new ArgumentNode("ARG",         ValueType.String),
                    new ArgumentNode("<myarg>",     ValueType.String),
                    new ArgumentNode("OPTIONALARG", ValueType.String),
                    new OptionNode  ("option",      ValueType.Bool),
                    new OptionNode  ("s",           ValueType.String),
                    new OptionNode  ("long",        ValueType.String),
                    new OptionNode  ("switch",      ValueType.Bool),
                    new ArgumentNode("FILE",        ValueType.List),
                    new CommandNode ("diff-command"),
                    new ArgumentNode("<myarg>",     ValueType.String),
                    new ArgumentNode("OPTIONALARG", ValueType.String),
                    new ArgumentNode("ARG",         ValueType.String),
                },
                actual);
        }
    }
}
