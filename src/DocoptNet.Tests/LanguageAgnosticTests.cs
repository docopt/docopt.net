using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;

namespace DocoptNet.Tests
{
    [TestFixture]
    public partial class LanguageAgnosticTests
    {
        public string Docopt(string doc, string cmdLine)
        {
            try
            {
                var arguments = new Docopt().Apply(doc, cmdLine);
                var dict = new Dictionary<string, object>();
                foreach (var argument in arguments)
                {
                    if (argument.Value == null)
                        dict[argument.Key] = null;
                    else if (argument.Value.IsList)
                    {
                        var l = new ArrayList();
                        foreach (var item in argument.Value.AsList)
                        {
                            l.Add(item is ValueObject { Value: var v } ? v : item);
                        }
                        dict[argument.Key] = l;
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
            if (expectedJson.StartsWith("{"))
            {
                var expectedDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(expectedJson);
                var actualDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(resultJson);
                Assert.AreEqual(expectedDict, actualDict);
            }
            else
            {
                var expected = JsonConvert.DeserializeObject(expectedJson).ToString();
                var actual = JsonConvert.DeserializeObject(resultJson).ToString();
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
