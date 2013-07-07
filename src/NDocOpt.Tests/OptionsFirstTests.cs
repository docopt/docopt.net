using System.Collections.Generic;
using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class OptionsFirstTests
    {
        [Test]
        public void Match_opt_first()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"--opt", new ValueObject(true)},
                    {"<args>", new ValueObject(new[]{"this", "that"})}
                };
            var actual = new Docopt().Apply("usage: prog [--opt] [<args>...]", "--opt this that");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Match_args_first()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"--opt", new ValueObject(true)},
                    {"<args>", new ValueObject(new[]{"this", "that"})}
                };
            var actual = new Docopt().Apply("usage: prog [--opt] [<args>...]", "this that --opt");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Check_options_first()
        {
            var expected = new Dictionary<string, ValueObject>
                {
                    {"--opt", new ValueObject(false)},
                    {"<args>", new ValueObject(new[]{"this", "that", "--opt"})}
                };
            var actual = new Docopt().Apply("usage: prog [--opt] [<args>...]", "this that --opt", optionsFirst: true);
            Assert.AreEqual(expected, actual);
        }
    }
}