#nullable enable

namespace DocoptNet
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    sealed partial class DocoptArgumentsAttribute : Attribute
    {
        public string? HelpConstName { get; set; }
    }
}

#if DOCOPTNET_INTERNAL

namespace DocoptNet
{
    using System.Runtime.CompilerServices;
    using Microsoft.CodeAnalysis;

    partial class DocoptArgumentsAttribute
    {
        public static DocoptArgumentsAttribute From(AttributeData data)
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

#endif // DOCOPTNET_INTERNAL
