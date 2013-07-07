using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class ShortOptionsErrorHandlingTests
    {
        [Test]
        public void Duplicate()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply("Usage: prog -x\nOptions: -x  this\n -x  that"));
        }

        [Test]
        public void Non_existent()
        {
            Assert.Throws<DocoptExitException>(
                () => new Docopt().Apply("Usage: prog", "-x"));
        }

        [Test]
        public void Wrong_opt_with_arg_spec()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply("Usage: prog -o\nOptions: -o ARG"));
        }

        [Test]
        public void Wrong_missing_arg()
        {
            Assert.Throws<DocoptExitException>(
                () => new Docopt().Apply("Usage: prog -o ARG\nOptions: -o ARG", "-o"));
        }
    }
}