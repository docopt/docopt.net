using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class AnyOptionsParameterTests
    {
        [Test]
        public void Error_long_with_arg()
        {
            Assert.Throws<DocoptInputErrorException>(
                () => new Docopt().Apply("usage: prog [options]", "-foo --bar --spam=eggs"));

        }

        [Test]
        public void Error_all_long()
        {
            Assert.Throws<DocoptInputErrorException>(
                () => new Docopt().Apply("usage: prog [options]", "--foo --bar --bar"));

        }

        [Test]
        public void Error_short_acc()
        {
            Assert.Throws<DocoptInputErrorException>(
                () => new Docopt().Apply("usage: prog [options]", "--bar --bar --bar -ffff"));

        }

        [Test]
        public void Error_all_long_with_arg()
        {
            Assert.Throws<DocoptInputErrorException>(
                () => new Docopt().Apply("usage: prog [options]", "--long=arg --long=another"));

        }
    }
}
