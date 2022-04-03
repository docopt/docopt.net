#nullable enable

namespace DocoptNet.Tests.Integration
{
    using System;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using static FlagArgs;

    public interface INavalFateArguments
    {
        public bool CmdShip { get; }
        public bool CmdNew { get; }
        public StringList ArgName { get; }
        public bool CmdMove { get; }
        public string? ArgX { get; }
        public string? ArgY { get; }
        public string OptSpeed { get; }
        public bool CmdShoot { get; }
        public bool CmdMine { get; }
        public bool CmdSet { get; }
        public bool CmdRemove { get; }
        public bool OptMoored { get; }
        public bool OptDrifting { get; }
        public bool OptHelp { get; }
        public bool OptVersion { get; }
    }

    [Flags]
    public enum FlagArgs
    {
        CmdShip     = 1 << 0,
        CmdNew      = 1 << 1,
        CmdMove     = 1 << 2,
        CmdShoot    = 1 << 3,
        CmdMine     = 1 << 4,
        CmdSet      = 1 << 5,
        CmdRemove   = 1 << 6,
        OptMoored   = 1 << 7,
        OptDrifting = 1 << 8,
    }

    public abstract class NavalFateTestsBase<T> where T : INavalFateArguments
    {
        protected NavalFateTestsBase(IHelpFeaturingParser<T> parser) => Parser = parser;

        IHelpFeaturingParser<T> Parser { get; }

        private IParser<T>.IResult Parse(string commandLine) =>
            (IParser<T>.IResult)Parser.Parse(commandLine.Split(' '));

        [TestCase("ship new foo bar baz", CmdShip | CmdNew, new[] { "foo", "bar", "baz" }, null, null, "10")]
        [TestCase("ship foo move 123 456", CmdShip | CmdMove, new[] { "foo" }, "123", "456", "10")]
        [TestCase("ship foo move 123 456 --speed=789", CmdShip | CmdMove, new[] { "foo" }, "123", "456", "789")]
        [TestCase("ship shoot 123 456", CmdShip | CmdShoot, new string[0], "123", "456", "10")]
        [TestCase("mine set 123 456", CmdMine | CmdSet, new string[0], "123", "456", "10")]
        [TestCase("mine remove 123 456", CmdMine | CmdRemove, new string[0], "123", "456", "10")]
        [TestCase("mine set 123 456 --moored", CmdMine | CmdSet | OptMoored, new string[0], "123", "456", "10")]
        [TestCase("mine set 123 456 --drifting", CmdMine | CmdSet | OptDrifting, new string[0], "123", "456", "10")]
        public void GoodUsage(string commandLine, FlagArgs flags, string[] names, string x, string y, string speed)
        {
            var args = ((IArgumentsResult<T>)Parse(commandLine)).Arguments;

            IResolveConstraint IsFlag(FlagArgs flag) => flags.HasFlag(flag) ? Is.True : Is.False;

            Assert.That(args.CmdShip, IsFlag(CmdShip));
            Assert.That(args.CmdNew, IsFlag(CmdNew));
            Assert.That(args.ArgName, Is.EqualTo(names));
            Assert.That(args.CmdMove, IsFlag(CmdMove));
            Assert.That(args.ArgX, Is.EqualTo(x));
            Assert.That(args.ArgY, Is.EqualTo(y));
            Assert.That(args.OptSpeed, Is.EqualTo(speed));
            Assert.That(args.CmdShoot, IsFlag(CmdShoot));
            Assert.That(args.CmdMine, IsFlag(CmdMine));
            Assert.That(args.CmdSet, IsFlag(CmdSet));
            Assert.That(args.CmdRemove, IsFlag(CmdRemove));
            Assert.That(args.OptMoored, IsFlag(OptMoored));
            Assert.That(args.OptDrifting, IsFlag(OptDrifting));
            Assert.That(args.OptHelp, Is.False);
            Assert.That(args.OptVersion, Is.False);
        }

        [TestCase("")]
        [TestCase("foo")]
        [TestCase("ship new")]
        [TestCase("SHIP new foo")]
        [TestCase("ship foo move")]
        [TestCase("ship foo move 123")]
        [TestCase("mine set remove 123 456")]
        public void BadUsage(string commandLine)
        {
            var result = (IInputErrorResult)Parse(commandLine);

            Assert.That(result.Usage, Is.EqualTo(NavalFateArguments.Usage));
        }

        [TestCase("-h")]
        [TestCase("--help")]
        public void Help(string commandLine)
        {
            var result = (IHelpResult)Parse(commandLine);

            Assert.That(result.Help, Is.EqualTo(NavalFateArguments.Help));
        }

        [Test]
        public void Version()
        {
            const string version = "1.2.3";

            var argv = new[] { "--version" };
            var result = (IVersionResult)Parser.WithVersion(version).Parse(argv);

            Assert.That(result.Version, Is.EqualTo(version));
        }
    }

    public sealed partial class NavalFateArguments : INavalFateArguments { }

    public class NavalFateTests : NavalFateTestsBase<NavalFateArguments>
    {
        public NavalFateTests() :
            base(NavalFateArguments.CreateParser()) { }
    }

    [DocoptArguments]
    public sealed partial class InlineNavalFateArguments : INavalFateArguments
    {
        public const string Help = @"Naval Fate.

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
    }

    public class InlineNavalFateTests : NavalFateTestsBase<InlineNavalFateArguments>
    {
        public InlineNavalFateTests() :
            base(InlineNavalFateArguments.CreateParser()) { }
    }

    namespace Nested
    {
        [DocoptArguments]
        public sealed partial class InlineNavalFateArguments : INavalFateArguments
        {
            const string Help = Integration.InlineNavalFateArguments.Help;
        }

        public class InlineNavalFateTests : NavalFateTestsBase<Integration.InlineNavalFateArguments>
        {
            public InlineNavalFateTests() :
                base(Integration.InlineNavalFateArguments.CreateParser()) { }
        }
    }

    public static partial class NavalFateProgram
    {
        [DocoptArguments]
        public sealed partial class Arguments : INavalFateArguments
        {
            const string Help = InlineNavalFateArguments.Help;
        }
    }

    public class NestedNavalFateTests : NavalFateTestsBase<NavalFateProgram.Arguments>
    {
        public NestedNavalFateTests() :
            base(NavalFateProgram.Arguments.CreateParser()) { }
    }
}
