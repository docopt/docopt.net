namespace DocoptNet.Tests;

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

[TestFixture]
public class ParserApiTests
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

    static IParser<IDictionary<string, Value>>.IResult Parse(string commandLine) =>
        Docopt.CreateParser(Help).WithVersion(Version).Parse(Args.Parse(commandLine).List);

    [Test]
    public void Parse_Switch_Args()
    {
        switch (Parse("ship new foo bar"))
        {
            case IArgumentsResult<IDictionary<string, Value>> { Arguments: var args }:
                Assert.True(args["ship"].IsTrue);
                Assert.True(args["new"].IsTrue);
                Assert.That((StringList)args["<name>"], Is.EqualTo(new[] { "foo", "bar" }));
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

    [TestCase("foobar", "Invalid usage.")]
    [TestCase("ship move 123 456 --speed", "--speed requires an argument")]
    public void Parse_Switch_Error(string commandLine, string expectedError)
    {
        switch (Parse(commandLine))
        {
            case IInputErrorResult result:
                Assert.That(result.Error, Is.EqualTo(expectedError));
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
        Assert.That((StringList)args["<name>"], Is.EqualTo(new[] { "foo", "bar" }));
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

    [TestCase("foobar", "Invalid usage.")]
    [TestCase("ship move 123 456 --speed", "--speed requires an argument")]
    public void Parse_Match_Error(string commandLine, string expectedError)
    {
        var result = Parse(commandLine);
        var error =
            result.Match(_ => throw new NUnitException(),
                         _ => throw new NUnitException(),
                         _ => throw new NUnitException(),
                         error => error);
        Assert.That(error.Error, Is.EqualTo(expectedError));
        Assert.That(error.Usage, Is.Not.Empty);
    }

    public class ParserTests
    {
        static readonly IParser<IDictionary<string, Value>> Parser =
            Docopt.CreateParser(Help).WithVersion(Version);

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List)
                             .Match(args => args,
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That((StringList)args["<name>"], Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void Match_Help()
        {
            var result = Parser.Parse(Args.Parse("foobar --help").List);
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
            var result = Parser.Parse(Args.Parse("foobar --version").List);
            var version =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             version => version,
                             _ => throw new NUnitException());
            Assert.That(version.Version, Is.EqualTo(Version));
        }

        [TestCase("foobar", "Invalid usage.")]
        [TestCase("ship move 123 456 --speed", "--speed requires an argument")]
        public void Match_Error(string commandLine, string expectedError)
        {
            var result = Parser.Parse(Args.Parse(commandLine).List);
            var error =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo(expectedError));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }

    public class HelpFeaturingParserTests
    {
        static readonly IHelpFeaturingParser<IDictionary<string, Value>> Parser = Docopt.CreateParser(Help);

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List)
                             .Match(args => args,
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That((StringList)args["<name>"], Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void Match_Help()
        {
            var result = Parser.Parse(Args.Parse("foobar --help").List);
            var help =
                result.Match(_ => throw new NUnitException(),
                             help => help,
                             _ => throw new NUnitException());
            Assert.That(help.Help, Is.EqualTo(Help));
        }

        [TestCase("foobar", "Invalid usage.")]
        [TestCase("ship move 123 456 --speed", "--speed requires an argument")]
        public void Match_Error(string commandLine, string expectedError)
        {
            var result = Parser.Parse(Args.Parse(commandLine).List);
            var error =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo(expectedError));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }

    public class VersionFeaturingParserTests
    {
        static readonly IVersionFeaturingParser<IDictionary<string, Value>> Parser =
            Docopt.CreateParser(Help).DisableHelp().WithVersion(Version);

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List)
                             .Match(args => args,
                                    _ => throw new NUnitException(),
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That((StringList)args["<name>"], Is.EqualTo(new[] { "foo", "bar" }));
        }

        [Test]
        public void Match_Version()
        {
            var result = Parser.Parse(Args.Parse("foobar --version").List);
            var version =
                result.Match(_ => throw new NUnitException(),
                             version => version,
                             _ => throw new NUnitException());
            Assert.That(version.Version, Is.EqualTo(Version));
        }

        [TestCase("foobar", "Invalid usage.")]
        [TestCase("ship move 123 456 --speed", "--speed requires an argument")]
        public void Match_Error(string commandLine, string expectedError)
        {
            var result = Parser.Parse(Args.Parse(commandLine).List);
            var error =
                result.Match(_ => throw new NUnitException(),
                             _ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo(expectedError));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }

    public class BaselineParserTests
    {
        static readonly IBaselineParser<IDictionary<string, Value>> Parser =
            Docopt.CreateParser(Help).DisableHelp();

        [Test]
        public void Match_Args()
        {
            var args = Parser.Parse(Args.Parse("ship new foo bar").List)
                             .Match(args => args,
                                    _ => throw new NUnitException());
            Assert.True(args["ship"].IsTrue);
            Assert.True(args["new"].IsTrue);
            Assert.That((StringList)args["<name>"], Is.EqualTo(new[] { "foo", "bar" }));
        }

        [TestCase("foobar", "Invalid usage.")]
        [TestCase("ship move 123 456 --speed", "--speed requires an argument")]
        public void Match_Error(string commandLine, string expectedError)
        {
            var result = Parser.Parse(Args.Parse(commandLine).List);
            var error =
                result.Match(_ => throw new NUnitException(),
                             error => error);
            Assert.That(error.Error, Is.EqualTo(expectedError));
            Assert.That(error.Usage, Is.Not.Empty);
        }
    }
}
