using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DocoptNet;
using Newtonsoft.Json;

namespace Testee
{
    internal class Program
    {
        public static string Docopt(string doc, string[] cmdLine)
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
                        foreach (var v in argument.Value.AsList)
                        {
                            if (v is ValueObject)
                                l.Add(((v) as ValueObject).Value);
                            else
                                l.Add(v);
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

        private static void Main(string[] args)
        {
            Stream inputStream = Console.OpenStandardInput();
            var bytes = new byte[100];
            var sb = new StringBuilder();
            int outputLength = inputStream.Read(bytes, 0, 100);
            while (outputLength > 0)
            {
                char[] chars = Encoding.UTF8.GetChars(bytes, 0, outputLength);
                sb.Append(chars);
                outputLength = inputStream.Read(bytes, 0, 100);
            }
            var doc = sb.ToString();
            var s = Docopt(doc, args);
            Console.WriteLine(s);
        }
    }
}
