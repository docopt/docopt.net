using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using DocoptNet;

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
                        foreach (var item in argument.Value.AsList)
                        {
                            l.Add(item is ValueObject { Value: var v } ? v : item);
                        }
                        dict[argument.Key] = l;
                    }
                    else
                        dict[argument.Key] = argument.Value.Value;
                }
                return JsonSerializer.Serialize(dict, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    //
                    // Compared to the default encoder, the UnsafeRelaxedJsonEscaping encoder is
                    // more permissive about allowing characters to pass through unescaped:
                    //
                    // - It doesn't escape HTML-sensitive characters such as "<", ">", "&", and "'".
                    // - It doesn't offer any additional defense-in-depth protections against XSS or
                    //   information disclosure attacks, such as those which might result from the
                    //   client and server disagreeing on the charset.
                    //
                    // Since this is not expected to be used in the web context, the more relaxed
                    // escaping is acceptable and less surprising.
                    //
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                });
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
