namespace DocoptNet.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public class LanguageAgnosticTests
    {
        [Test]
        [TestCaseSource(nameof(Docopt), new object[] { "testcases.docopt" })]
        public void Test(string doc, string cmdLine, string expected)
        {
            string actual;

            try
            {
                var arguments = new Docopt().Apply(doc, cmdLine);
                var dict = new Dictionary<string, object>();
                foreach (var argument in arguments)
                    dict[argument.Key] = argument.Value.Box;
                actual = JsonConvert.SerializeObject(dict);
            }
            catch (Exception)
            {
                actual = "\"user-error\"";
            }

            if (expected.StartsWith("{"))
            {
                var expectedDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(expected);
                var actualDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(actual);
                Assert.AreEqual(expectedDict, actualDict);
            }
            else
            {
                var expected1 = JsonConvert.DeserializeObject(expected).ToString();
                var actual1 = JsonConvert.DeserializeObject(actual).ToString();
                Assert.AreEqual(expected1, actual1);
            }
        }

        /// <remarks>
        /// If <paramref name="path"/> is a relative path then it is resolved against
        /// <seealso cref="TestContext.WorkDirectory"/>.
        /// </remarks>

        static IEnumerable<TestCaseData> Docopt(string path)
        {
            // quick and dirty "*.docopt" to test cases conversion

            var fixtures = File.ReadAllText(Path.Combine(TestContext.CurrentContext.WorkDirectory, path));
            fixtures = Regex.Replace(fixtures, @"#.*$", string.Empty, RegexOptions.Multiline);
            foreach (var fixture in Regex.Split(fixtures, @"r""{3}"))
            {
                const string quote = "\"";
                var (doc, _, body) = fixture.Partition(quote + quote + quote);
                foreach (var @case in body.Split('$').Skip(1))
                {
                    var (argv, _, expect) = @case.Trim().Partition("\n");
                    string prog;
                    (prog, _, argv) = argv.Trim().Partition(" ");
                    Debug.Assert(prog == "prog", prog);
                    yield return new TestCaseData(doc, argv, expect);
                }
            }
        }
    }
}
