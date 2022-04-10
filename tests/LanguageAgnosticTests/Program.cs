// Licensed under terms of MIT license (see LICENSE-MIT)
// Copyright 2013 Dinh Doan Van Bien, 2021 Atif Aziz

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using DocoptNet;

var doc = Console.In.ReadToEnd();
Console.WriteLine(Docopt(doc, args));

static string Docopt(string doc, ICollection<string> cmdLine)
{
    try
    {
        var arguments = new Docopt().Apply(doc, cmdLine)!;
        var dict = new Dictionary<string, object?>();
        foreach (var argument in arguments)
        {
            if (argument.Value.IsList)
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
