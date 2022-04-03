#region Copyright (c) 2021 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

#nullable enable

namespace DocoptNet.CodeGeneration
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    static partial class Extensions
    {
        /// <remarks>
        /// Parents are returned in order of nearest to furthest ancestry.
        /// </remarks>
        public static IEnumerable<TypeDeclarationSyntax> GetParents(this BaseTypeDeclarationSyntax syntax)
        {
            for (var tds = syntax.Parent as TypeDeclarationSyntax;
                 tds is not null;
                 tds = tds.Parent as TypeDeclarationSyntax)
            {
                yield return tds;
            }
        }
    }

    // Inspiration & credit:
    // https://github.com/devlooped/ThisAssembly/blob/43eb32fa24c25ddafda1058a53857ea3e305296a/src/GeneratorExtension.cs

    partial class Extensions
    {
        public static void LaunchDebuggerIfFlagged(this GeneratorExecutionContext context,
                                                   string generatorName) =>
            context.AnalyzerConfigOptions.GlobalOptions.LaunchDebuggerIfFlagged(generatorName);

        public static void LaunchDebuggerIfFlagged(this AnalyzerConfigOptions options,
                                                   string generatorName)
        {
            if (options.IsFlagged("build_property.DebugSourceGenerators") ||
                options.IsFlagged("build_property.Debug" + generatorName))
            {
                Debugger.Launch();
            }
        }

        public static bool IsFlagged(this AnalyzerConfigOptions options, string name) =>
            options.TryGetValue(name, out var s) && bool.TryParse(s, out var flag) && flag;
    }
}

#region (c) West Wind Technologies, 2008 - 2009
/*
 **************************************************************
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************
 */
#endregion

namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Text;

    // Following extensions were adapted from:
    // https://weblog.west-wind.com/posts/2018/Nov/30/Returning-an-XML-Encoded-String-in-NET

    partial class Extensions
    {
        public static string EncodeXmlText(this string str) => str.EncodeXmlText(attribute: false);
        public static string EncodeXmlAttributeValue(this string str) => str.EncodeXmlText(attribute: true);

        static string EncodeXmlText(this string str, bool attribute)
        {
            StringBuilder? sb = null;

            StringBuilder StringBuilder() => sb ??= new StringBuilder(str.Length);
            void Append(string s) => StringBuilder().Append(s);
            void AppendChar(char ch) => StringBuilder().Append(ch);

            foreach (var ch in str)
            {
                switch (attribute, ch)
                {
                    case (_, '<')    : Append("&lt;"); break;
                    case (_, '>')    : Append("&gt;"); break;
                    case (_, '&')    : Append("&amp;"); break;
                    case (true, '"') : Append("&quot;"); break;
                    case (true, '\''): Append("&apos;"); break;
                    case (true, '\n'): Append("&#xA;"); break;
                    case (true, '\r'): Append("&#xD;"); break;
                    case (true, '\t'): Append("&#x9;"); break;
                    case (_, < ' ')  : throw new ArgumentException($"Character U+{(int)ch:X4} is forbidden in XML.", nameof(str));
                    default          : AppendChar(ch); break;
                }
            }

            return sb?.ToString() ?? str;
        }
    }
}
