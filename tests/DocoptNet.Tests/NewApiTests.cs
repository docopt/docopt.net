namespace DocoptNet.Tests;

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

[TestFixture]
public class NewApiTests
{
    const string Help = @"Naval Fate.

    Usage:
      naval_fate.exe ship new <name>...
      naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
      naval_fate.exe ship shoot <x> <y>
      naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
      naval_fate.exe (-h | --help)
      naval_fate.exe --version

    Options:
      -h --help     Show this screen.
      --version     Show version.
      --speed=<kn>  Speed in knots [default: 10].
      --moored      Moored (anchored) mine.
      --drifting    Drifting mine.
";

    const string Version = "Naval Fate 2.0";

    static IParser2<IDictionary<string, ValueObject>>.IResult Parse(string commandLine) =>
        Docopt.Parse2(Help, Args.Parse(commandLine).List, Docopt.ParseFlags.None, Version);

    [Test]
    public void Parse_Switch_Args()
    {
        switch (Parse("ship new foo bar"))
        {
            case IArgumentsResult<IDictionary<string, ValueObject>> { Arguments: var args }:
                Assert.True(args["ship"].IsTrue);
                Assert.True(args["new"].IsTrue);
                Assert.That(args["<name>"].AsList, Is.EqualTo(new[] { "foo", "bar" }));
                break;
            case var result:
                Assert.Fail("Unexpected result: {0}", result);
                break;
        }
    }

    [Test]
    public void Parse_Switch_Help()
    {
        switch (Parse("foobar --help"))
        {
            case IHelpResult result:
                Assert.That(result.Help, Is.EqualTo(Help));
                break;
            case var result:
                Assert.Fail("Unexpected result: {0}", result);
                break;
        }
    }

    [Test]
    public void Parse_Switch_Version()
    {
        switch (Parse("foobar --version"))
        {
            case IVersionResult result:
                Assert.That(result.Version, Is.EqualTo(Version));
                break;
            case var result:
                Assert.Fail("Unexpected result: {0}", result);
                break;
        }
    }

    [Test]
    public void Parse_Switch_Error()
    {
        switch (Parse("foobar"))
        {
            case IInputErrorResult result:
                Assert.That(result.Error, Is.EqualTo("Input error."));
                Assert.That(result.Usage, Is.Not.Empty);
                break;
            case var result:
                Assert.Fail("Unexpected result: {0}", result);
                break;
        }
    }

    [Test]
    public void Parse_Match_Args()
    {
        var result = Parse("ship new foo bar");
        var args =
            result.Match(args => args,
                         _ => throw new NUnitException(),
                         _ => throw new NUnitException(),
                         _ => throw new NUnitException());
        Assert.True(args["ship"].IsTrue);
        Assert.True(args["new"].IsTrue);
        Assert.That(args["<name>"].AsList, Is.EqualTo(new[] { "foo", "bar" }));
    }

    [Test]
    public void Parse_Match_Help()
    {
        var result = Parse("foobar --help");
        var help =
            result.Match(_ => throw new NUnitException(),
                         help => help,
                         _ => throw new NUnitException(),
                         _ => throw new NUnitException());
        Assert.That(help.Help, Is.EqualTo(Help));
    }

    [Test]
    public void Parse_Match_Version()
    {
        var result = Parse("foobar --version");
        var version =
            result.Match(_ => throw new NUnitException(),
                         _ => throw new NUnitException(),
                         version => version,
                         _ => throw new NUnitException());
        Assert.That(version.Version, Is.EqualTo(Version));
    }

    [Test]
    public void Parse_Match_Error()
    {
        var result = Parse("foobar");
        var error =
            result.Match(_ => throw new NUnitException(),
                         _ => throw new NUnitException(),
                         _ => throw new NUnitException(),
                         error => error);
        Assert.That(error.Error, Is.EqualTo("Input error."));
        Assert.That(error.Usage, Is.Not.Empty);
    }

    public class Parser2
    {
        static readonly IParser2<IDictionary<string, ValueObject>> Parser =
            Docopt.Parser2(Help).WithVersion(Version);

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List, Docopt.ParseFlags.None)
                             .Match(args => args,
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That(args["<name>"].AsList, Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void Match_Help()
        {
            var result = Parser.Parse(Args.Parse("foobar --help").List, Docopt.ParseFlags.None);
            var help =
                result.Match(_ => throw new NUnitException(),
                             help => help,
                             _ => throw new NUnitException(),
                             _ => throw new NUnitException());
            Assert.That(help.Help, Is.EqualTo(Help));
        }

        [Test]
        public void Match_Version()
        {
            var result = Parser.Parse(Args.Parse("foobar --version").List, Docopt.ParseFlags.None);
            var version =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             version => version,
                             _ => throw new NUnitException());
            Assert.That(version.Version, Is.EqualTo(Version));
        }

        [Test]
        public void Match_Error()
        {
            var result = Parser.Parse(Args.Parse("foobar").List, Docopt.ParseFlags.None);
            var error =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo("Input error."));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }

    public class ParserWithHelpSupport
    {
        static readonly IParserWithHelpSupport<IDictionary<string, ValueObject>> Parser = Docopt.Parser2(Help);

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List, Docopt.ParseFlags.None)
                             .Match(args => args,
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That(args["<name>"].AsList, Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void Match_Help()
        {
            var result = Parser.Parse(Args.Parse("foobar --help").List, Docopt.ParseFlags.None);
            var help =
                result.Match(_ => throw new NUnitException(),
                             help => help,
                             _ => throw new NUnitException());
            Assert.That(help.Help, Is.EqualTo(Help));
        }

        [Test]
        public void Match_Error()
        {
            var result = Parser.Parse(Args.Parse("foobar").List, Docopt.ParseFlags.None);
            var error =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo("Input error."));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }

    public class ParserWithVersionSupport
    {
        static readonly IParserWithVersionSupport<IDictionary<string, ValueObject>> Parser =
            Docopt.Parser2(Help).DisableHelp().WithVersion(Version);

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List, Docopt.ParseFlags.None)
                             .Match(args => args,
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That(args["<name>"].AsList, Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void Match_Version()
        {
            var result = Parser.Parse(Args.Parse("foobar --version").List, Docopt.ParseFlags.None);
            var version =
                result.Match(_ => throw new NUnitException(),
                             version => version,
                             _ => throw new NUnitException());
            Assert.That(version.Version, Is.EqualTo(Version));
        }

        [Test]
        public void Match_Error()
        {
            var result = Parser.Parse(Args.Parse("foobar").List, Docopt.ParseFlags.None);
            var error =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo("Input error."));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }

    public class BasicParser
    {
        static readonly IBasicParser<IDictionary<string, ValueObject>> Parser =
            Docopt.Parser2(Help).DisableHelp();

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List, Docopt.ParseFlags.None)
                             .Match(args => args,
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That(args["<name>"].AsList, Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void Match_Error()
        {
            var result = Parser.Parse(Args.Parse("foobar").List, Docopt.ParseFlags.None);
            var error =
                result.Match(_ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo("Input error."));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }
}
