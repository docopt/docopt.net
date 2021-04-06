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

namespace DocoptNet.CodeGeneration
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    // Inspiration & credit:
    // https://github.com/devlooped/ThisAssembly/blob/43eb32fa24c25ddafda1058a53857ea3e305296a/src/GeneratorExtension.cs

    static class Extensions
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

        public static string ToInvariantString<T>(this T formattable) where T : IFormattable =>
            formattable.ToString(null, CultureInfo.InvariantCulture);
    }
}
