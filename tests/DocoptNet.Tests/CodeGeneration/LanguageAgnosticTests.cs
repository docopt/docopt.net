#nullable enable

namespace DocoptNet.Tests.CodeGeneration
{
    using System;
    using System.Collections.Generic;
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
                        new object[] { "testcases.docopt" })]
        public void Test(string doc, string cmdLine, string expected)
        {
            const string main = @"
using System;
using System.Collections;
using System.Collections.Generic;
using DocoptNet.Generated;
using Newtonsoft.Json;

public partial class Program
{
    readonly IDictionary<string, Value> _args;

    public Program(IList<string> argv)
    {
        var arguments = ProgramArguments.Apply(argv, help: true, version: null, optionsFirst: false, exit: false);
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
";
            Assembly assembly;

            try
            {
                assembly = SourceGeneratorTests.GenerateProgram(doc, main);
            }
            catch (Exception e)
            {
                static int ParseInt(string s) => int.Parse(s, NumberStyles.None, CultureInfo.InvariantCulture);
                var (errorLineNumber, _) =
                    e is AssertionException
                    && Regex.Match(e.Message, @"\(([1-9][0-9]*),([1-9][0-9]*)\):\s*error\b") is { Success: true, Groups: {} groups }
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

            if (expected.StartsWith("{"))
            {
                var expectedDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(expected);
                var actualDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(actual);
                Assert.That(actualDict, Is.EquivalentTo(expectedDict));
            }
            else
            {
                var expected1 = JsonConvert.DeserializeObject(expected).ToString();
                var actual1 = JsonConvert.DeserializeObject(actual).ToString();
                Assert.AreEqual(expected1, actual1);
            }
        }
    }
}
