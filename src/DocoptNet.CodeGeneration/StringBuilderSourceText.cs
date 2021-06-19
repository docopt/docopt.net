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
    using System.Text;
    using Microsoft.CodeAnalysis.Text;

    sealed class StringBuilderSourceText : SourceText
    {
        readonly StringBuilder _builder;

        public StringBuilderSourceText(StringBuilder builder, Encoding? encoding = null) =>
            (_builder, Encoding) = (builder, encoding);

        public override Encoding? Encoding { get; }
        public override int Length => _builder.Length;
        public override char this[int position] => _builder[position];

        public override string ToString(TextSpan span) => _builder.ToString(span.Start, span.Length);

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
            _builder.CopyTo(sourceIndex, destination, destinationIndex, count);
    }
}
