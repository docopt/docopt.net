using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class LongOptionsErrorHandlingTests
    {
        [Test]
        public void Non_existent()
        {
            Assert.Throws<DocoptInputErrorException>(() =>
                                               new Docopt().Apply("Usage: prog", "--non-existent")
                );
        }

        [Test]
        public void Wrong_long_opt()
        {
            Assert.Throws<DocoptInputErrorException>(
                () => new Docopt().Apply(
                    "Usage: prog [--version --verbose]\n" +
                    "Options: --version\n" +
                    " --verbose", "--ver")
                );
        }

        [Test]
        public void Missing_long_opt_arg_spec()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply(
                    "Usage: prog --long\n" +
                    "Options: --long ARG\n", "")
                );
        }

        [Test]
        public void Missing_long_opt_arg()
        {
            Assert.Throws<DocoptInputErrorException>(
                () => new Docopt().Apply(
                    "Usage: prog --long ARG\n" +
                    "Options: --long ARG\n", "--long")
                );
        }

        [Test]
        public void Missing_long_opt_arg_spec2()
        {
            Assert.Throws<DocoptLanguageErrorException>(
                () => new Docopt().Apply(
                    "Usage: prog --long=ARG\n" +
                    "Options: --long\n", "")
                );
        }

        [Test]
        public void Unexpected_arg()
        {
            Assert.Throws<DocoptInputErrorException>(
                () => new Docopt().Apply(
                    "Usage: prog --long\n" +
                    "Options: --long\n", "--long=ARG")
                );
        }
    }
}