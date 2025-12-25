#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using DocoptNet.CodeGeneration;
    using Microsoft.CodeAnalysis.Text;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public class LanguageAgnosticTests
    {
        [Test]
        [TestCaseSource(typeof(Tests.LanguageAgnosticTests),
                        nameof(Tests.LanguageAgnosticTests.Docopt),
                        ["testcases.docopt"])]
        public void Test(string doc, string cmdLine, string expected)
        {
            const string main = """
                using System;
                using System.Collections;
                using System.Collections.Generic;
                using DocoptNet;
                using DocoptNet.Internals;
                using Newtonsoft.Json;

                public partial class Program
                {
                    public Program(IList<string> argv)
                    {
                        var arguments = ProgramArguments.CreateParser().Parse(argv) switch
                        {
                            IArgumentsResult<ProgramArguments> { Arguments: var args } => args,
                            IInputErrorResult => throw new DocoptInputErrorException(),
                            _ => throw new(),
                        };
                        var dict = new Dictionary<string, object>();
                        foreach (var (name, value) in arguments)
                        {
                            if (value is StringList items)
                            {
                                var l = new ArrayList();
                                foreach (var item in items)
                                    l.Add(item);
                                dict[name] = l;
                            }
                            else
                                dict[name] = value;
                        }
                        Json = JsonConvert.SerializeObject(dict);
                    }

                    public string Json { get; }
                }

                """;

            Assembly assembly;

            try
            {
                assembly = SourceGeneratorTests.GenerateProgram(("Program.docopt.txt", SourceText.From(doc)),
                                                                ("Main.cs", SourceText.From(main)));
            }
            catch (Exception e)
            {
                static int ParseInt(string s) => int.Parse(s, NumberStyles.None, CultureInfo.InvariantCulture);
                var (errorLineNumber, _) =
                    e is AssertionException
                    && Regex.Match(e.Message, @"\(([1-9][0-9]*),([1-9][0-9]*)\):\s*error\b") is { Success: true, Groups: var groups }
                    ? (ParseInt(groups[1].Value), ParseInt(groups[2].Value))
                    : default;
                var sourceText = SourceGenerator.Generate(null, "Program", SourceText.From(doc));
                foreach (var line in sourceText.Lines)
                {
                    var lineNumber = line.LineNumber + 1; // convert zero-base to one-based line number
                    Console.WriteLine($"{(lineNumber == errorLineNumber ? '!' : ' ')}{lineNumber,5}: {line}");
                }

                throw;
            }
            var type = assembly.GetType("Program")!;
            Assert.That(type, Is.Not.Null);

            string actual;
            try
            {
                var program = Activator.CreateInstance(type, (IList<string>)cmdLine.Split(' ', StringSplitOptions.RemoveEmptyEntries))!;
                Assert.That(program, Is.Not.Null);
                actual = (string)((dynamic)program).Json;
            }
            catch (TargetInvocationException e) when (e.GetBaseException().GetType().Name.StartsWith("Docopt", StringComparison.OrdinalIgnoreCase))
            {
                actual = "\"user-error\"";
            }

            if (expected is ['{', ..])
            {
                var expectedDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(expected);
                Debug.Assert(expectedDict is not null);
                var actualDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(actual);
                Assert.That(actualDict, Is.EquivalentTo(expectedDict));
            }
            else
            {
                var expected1 = JsonConvert.DeserializeObject(expected)!.ToString();
                var actual1 = JsonConvert.DeserializeObject(actual)!.ToString();
                Assert.That(actual1, Is.EqualTo(expected1));
            }
        }
    }
}
