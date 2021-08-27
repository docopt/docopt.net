#region Copyright (c) .NET Foundation and Contributors. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
#endregion

// Source: https://github.com/dotnet/runtime/blob/804a933e2b699e10391e7f8b4ccbbbfad41bfefc/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/SwitchExpressionException.cs

using System.Runtime.Serialization;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Indicates that a switch expression that was non-exhaustive failed to match its input
    /// at runtime, e.g. in the C# 8 expression <code>3 switch { 4 => 5 }</code>.
    /// The exception optionally contains an object representing the unmatched value.
    /// </summary>
    [Serializable]
    sealed class SwitchExpressionException : InvalidOperationException
    {
        public SwitchExpressionException() :
            this((string?)null) { }

        public SwitchExpressionException(Exception? innerException) :
            this(null, innerException) { }

        public SwitchExpressionException(object? unmatchedValue) :
            this() => UnmatchedValue = unmatchedValue;

        public SwitchExpressionException(string? message) :
            base(message, null) { }

        public SwitchExpressionException(string? message, Exception? innerException) :
            base(message ?? "Non-exhaustive switch expression failed to match its input.", innerException) { }

        SwitchExpressionException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            UnmatchedValue = info.GetValue(nameof(UnmatchedValue), typeof(object));
        }

        public object? UnmatchedValue { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(UnmatchedValue), UnmatchedValue, typeof(object));
        }

        public override string Message
        {
            get
            {
                if (UnmatchedValue is null)
                    return base.Message;

                var valueMessage = $"Unmatched value was {UnmatchedValue}.";
                return base.Message + Environment.NewLine + valueMessage;
            }
        }
    }
}
