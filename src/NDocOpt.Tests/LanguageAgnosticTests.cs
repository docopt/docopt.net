using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;

namespace NDocOpt.Tests
{
    [TestFixture]
    public partial class LanguageAgnosticTests
    {
        public string DocOpt(string doc, string cmdLine)
        {
            try
            {
                var arguments = new DocOpt().Apply(doc, cmdLine);
                var dict = new Dictionary<string, object>();
                foreach (var argument in arguments)
                {
                    if (argument.Value == null)
                        dict[argument.Key] = null;
                    else if (argument.Value.IsList)
                    {
                        var v = (argument.Value.Value as ICollection<object>).Select(x => ((ValueObject) x).Value);
                        dict[argument.Key] = v;
                    }
                    else
                        dict[argument.Key] = argument.Value.Value;
                }
                return JsonConvert.SerializeObject(dict);
            }
            catch (Exception)
            {
                return "\"user-error\"";
            }
        }

        public void CheckResult(string expectedJson, string resultJson)
        {
            var expected = JsonConvert.DeserializeObject(expectedJson).ToString();
            var actual = JsonConvert.DeserializeObject(resultJson).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}