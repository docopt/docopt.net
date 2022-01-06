namespace DocoptNet.Tests.Integration
{
    using System;
    using System.Diagnostics;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using static NavalFateTests.FlagArgs;

    public class NavalFateTests
    {
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
            var args = NavalFateArguments.Apply(commandLine.Split(' '));

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
            var ex = Assert.Throws<DocoptInputErrorException>(() => NavalFateArguments.Apply(commandLine.Split(' ')));

            Debug.Assert(ex is not null);
            Assert.That(ex.Message, Is.EqualTo(NavalFateArguments.Usage));
        }

        [TestCase("-h")]
        [TestCase("--help")]
        public void Help(string commandLine)
        {
            var ex = Assert.Throws<DocoptExitException>(() => NavalFateArguments.Apply(commandLine.Split(' ')));

            Debug.Assert(ex is not null);
            Assert.That(ex.Message, Is.EqualTo(NavalFateArguments.Help));
        }

        [Test]
        public void Version()
        {
            const string version = "1.2.3";

            var args = new[] { "--version" };
            var ex = Assert.Throws<DocoptExitException>(() => NavalFateArguments.Apply(args, version: version));

            Debug.Assert(ex is not null);
            Assert.That(ex.Message, Is.EqualTo(version));
        }
    }
}
