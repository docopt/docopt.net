using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDocOpt;
using Newtonsoft.Json;

namespace Testee
{
    internal class Program
    {
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
            try
            {
                var arguments = new DocOpt().Apply(doc, args);
                var dict = new Dictionary<string, object>();
                foreach (var argument in arguments)
                {
                    if (argument.Value.IsList)
                    {
                        var v = (argument.Value.Value as ICollection<object>).Select(x => ((ValueObject)x).Value);
                        dict[argument.Key] = v;
                    }
                    else
                        dict[argument.Key] = argument.Value.Value;
                }
                Console.WriteLine(JsonConvert.SerializeObject(dict));
            }
            catch (Exception e)
            {
                Console.WriteLine("\"user-error\"");
            }
            
        }
    }
}