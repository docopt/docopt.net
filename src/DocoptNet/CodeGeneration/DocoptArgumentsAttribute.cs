#nullable enable

namespace DocoptNet
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Class)]
    sealed partial class DocoptArgumentsAttribute : Attribute
    {
        public string? HelpConstName { get; set; }

        internal static DocoptArgumentsAttribute From(AttributeData data)
        {
            var attribute = new DocoptArgumentsAttribute();
            foreach (var arg in data.NamedArguments)
            {
                var value = arg.Value.Value;
                switch (arg.Key)
                {
                    case nameof(HelpConstName): attribute.HelpConstName  = (string?)value; break;
                    case var key: throw new SwitchExpressionException(key);
                }
            }

            return attribute;
        }
    }
}
