namespace DocoptNet.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class PatterEitherTests
    {
        [Test]
        public void Should_transform_option()
        {
            Assert.AreEqual(
                new Either(new Required(new Option("-a"))),
                Pattern.Transform(new Option("-a"))
                );
        }

        [Test]
        public void Should_transform_arg()
        {
            Assert.AreEqual(
                new Either(new Required(new Argument("A"))),
                Pattern.Transform(new Argument("A"))
                );
        }

        [Test]
        public void Should_distribute_opt()
        {
            Assert.AreEqual(
                new Either(
                    new Required(new Option("-a"), new Option("-c")),
                    new Required(new Option("-b"), new Option("-c"))),
                Pattern.Transform(new Required(
                                      new Either(new Option("-a"), new Option("-b")), new Option("-c")))
                );
        }

        [Test]
        public void Should_distribute_optional()
        {
            Assert.AreEqual(
                new Either(
                    new Required(new Option("-b"), new Option("-a")),
                    new Required(new Option("-c"), new Option("-a"))),
                Pattern.Transform(new Optional(
                                      new Option("-a"), new Either(new Option("-b"), new Option("-c"))))
                );
        }

        [Test]
        public void Should_split_either()
        {
            Assert.AreEqual(
                new Either(
                    new Required(new Option("-x")),
                    new Required(new Option("-y")),
                    new Required(new Option("-z"))
                    ),
                Pattern.Transform(new Either(new Option("-x"),
                                             new Either(new Option("-y"), new Option("-z"))))
                );
        }

        [Test]
        public void Should_transform_oneormore_args()
        {
            Assert.AreEqual(
                new Either(new Required(new Argument("N"), new Argument("M"),
                                        new Argument("N"), new Argument("M"))),
                Pattern.Transform(new OneOrMore(new Argument("N"), new Argument("M")))
                );
        }
    }
}
