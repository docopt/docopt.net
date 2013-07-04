using NUnit.Framework;

namespace NDocOpt.Tests
{
    [TestFixture]
    public class LongOptionsErrorHandlingTests
    {
        [Test]
        public void Non_existent()
        {
            Assert.Throws<DocOptExitException>(() =>
                                               new DocOpt().Apply("Usage: prog", "--non-existent")
                );
        }

        [Test]
        public void Wrong_long_opt()
        {
            Assert.Throws<DocOptExitException>(
                () => new DocOpt().Apply(
                    "Usage: prog [--version --verbose]\n" +
                    "Options: --version\n" +
                    " --verbose", "--ver")
                );
        }

        [Test]
        public void Missing_long_opt_arg_spec()
        {
            Assert.Throws<DocOptLanguageErrorException>(
                () => new DocOpt().Apply(
                    "Usage: prog --long\n" +
                    "Options: --long ARG\n", "")
                );
        }

        [Test]
        public void Missing_long_opt_arg()
        {
            Assert.Throws<DocOptExitException>(
                () => new DocOpt().Apply(
                    "Usage: prog --long ARG\n" +
                    "Options: --long ARG\n", "--long")
                );
        }

        [Test]
        public void Missing_long_opt_arg_spec2()
        {
            Assert.Throws<DocOptLanguageErrorException>(
                () => new DocOpt().Apply(
                    "Usage: prog --long=ARG\n" +
                    "Options: --long\n", "")
                );
        }

        [Test]
        public void Unexpected_arg()
        {
            Assert.Throws<DocOptExitException>(
                () => new DocOpt().Apply(
                    "Usage: prog --long\n" +
                    "Options: --long\n", "--long=ARG")
                );
        }
    }
}