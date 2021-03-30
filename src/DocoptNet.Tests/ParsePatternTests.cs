using NUnit.Framework;

namespace DocoptNet.Tests
{
    [TestFixture]
    public class ParsePatternTests
    {
        private readonly Option[] _options = new[] {new Option("-h"), new Option("-v", "--verbose"), new Option("-f", "--file", 1)};

        [Test]
        public void Test_parse_pattern_one_optional_option()
        {
            Assert.AreEqual(new Required(new Optional(new Option("-h"))), Docopt.ParsePattern("[ -h ]", _options));
        }

        [Test]
        public void Test_parse_pattern_optional_oneormore_arg()
        {
            Assert.AreEqual(new Required(new Optional(new OneOrMore(new Argument("ARG")))),
                            Docopt.ParsePattern("[ ARG ... ]", _options));
        }

        [Test]
        public void Test_parse_pattern_either_options()
        {
            Assert.AreEqual(new Required(new Optional(new Either(new Option("-h"), new Option("-v", "--verbose")))),
                            Docopt.ParsePattern("[ -h | -v ]", _options));
        }

        [Test]
        public void Test_parse_pattern_either_options_and_optional_arg_opt()
        {
            Assert.AreEqual(new Required(new Required(
                                             new Either(new Option("-h"),
                                                        new Required(new Option("-v", "--verbose"),
                                                                     new Optional(new Option("-f", "--file", 1))
                                                            )
                                                 )
                                             )
                                ),
                            Docopt.ParsePattern("( -h | -v [ --file <f> ] )", _options));
        }

        [Test]
        public void Test_parse_pattern_optional_and_oneormore_arg()
        {
            Assert.AreEqual(new Required(new Required(
                                             new Either(new Option("-h"),
                                                        new Required(new Option("-v", "--verbose"),
                                                                     new Optional(new Option("-f", "--file", 1)),
                                                                     new OneOrMore(new Argument("N"))
                                                            )
                                                 )
                                             )
                                ),
                            Docopt.ParsePattern("(-h|-v[--file=<f>]N...)", _options));
        }

        [Test]
        public void Test_parse_pattern_mix_of_either()
        {
            Assert.AreEqual(new Required(new Required(new Either(
                                                          new Required(new Argument("N"),
                                                                       new Optional(new Either(new Argument("M"),
                                                                                               new Required(
                                                                                                   new Either(
                                                                                                       new Argument("K"),
                                                                                                       new Argument("L")))
                                                                                        )
                                                                           )
                                                              )
                                                          , new Required(new Argument("O"), new Argument("P"))))
                                ),
                            Docopt.ParsePattern("(N [M | (K | L)] | O P)", _options));
        }

        [Test]
        public void Test_parse_pattern_option_with_optional_arg()
        {
            Assert.AreEqual(new Required(new Optional(new Option("-h")), new Optional(new Argument("N"))),
                            Docopt.ParsePattern("[ -h ] [N]", _options));
        }

        [Test]
        public void Test_parse_pattern_options_shortcut()
        {
            Assert.AreEqual(new Required(new Optional(new OptionsShortcut())),
                            Docopt.ParsePattern("[options]", _options));
        }

        [Test]
        public void Test_parse_pattern_options_shortcut_with_arg()
        {
            Assert.AreEqual(new Required(new Optional(new OptionsShortcut()), new Argument("A")),
                            Docopt.ParsePattern("[options] A", _options));
        }

        [Test]
        public void Test_parse_pattern_options_shortcut_with_long()
        {
            Assert.AreEqual(new Required(new Option("-v", "--verbose"), new Optional(new OptionsShortcut())),
                            Docopt.ParsePattern("-v [options]", _options));
        }

        [Test]
        public void Test_parse_pattern_arg_upper_case()
        {
            Assert.AreEqual(new Required(new Argument("ADD")),
                            Docopt.ParsePattern("ADD", _options));
        }

        [Test]
        public void Test_parse_pattern_arg_between_pointy_brackets()
        {
            Assert.AreEqual(new Required(new Argument("<add>")),
                            Docopt.ParsePattern("<add>", _options));
        }

        [Test]
        public void Test_parse_pattern_command()
        {
            Assert.AreEqual(new Required(new Command("add")),
                            Docopt.ParsePattern("add", _options));
        }

    }
}
