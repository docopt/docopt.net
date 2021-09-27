#nullable enable

namespace DocoptNet
{
    using System;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Class)]
    sealed partial class DocoptArgumentsAttribute : Attribute
    {
        FieldAttributes _helpConstAccessibility = FieldAttributes.Public;

        public string? HelpFile { get; set; }
        public string? HelpConstName { get; set; }

        public FieldAttributes HelpConstAccessibility
        {
            get => _helpConstAccessibility;
            set
            {
                if ((value & (~FieldAttributes.FieldAccessMask | FieldAttributes.FamANDAssem)) != 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _helpConstAccessibility = value;
            }
        }

        public bool Help { get; set; }
        public string? Version { get; set; }
        public bool OptionsFirst { get; set; }
        public bool Exit { get; set; }
        public string? CommandPrefix { get; set; }
        public string? ArgumentPrefix { get; set; }
        public string? OptionPrefix { get; set; }
        public string? CommandSuffix { get; set; }
        public string? ArgumentSuffix { get; set; }
        public string? OptionSuffix { get; set; }
    }
}

#if DOCOPTNET_INTERNAL

namespace DocoptNet
{
    using System.Reflection;
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
                    case nameof(HelpFile)              : attribute.HelpFile       = (string?)value; break;
                    case nameof(Help)                  : attribute.Help           = value is true ; break;
                    case nameof(HelpConstName)         : attribute.HelpConstName  = (string?)value; break;
                    case nameof(HelpConstAccessibility): attribute.HelpConstAccessibility = (FieldAttributes)value!; break;
                    case nameof(Version)               : attribute.Version        = (string?)value; break;
                    case nameof(OptionsFirst)          : attribute.OptionsFirst   = value is true ; break;
                    case nameof(Exit)                  : attribute.Exit           = value is true ; break;
                    case nameof(CommandPrefix)         : attribute.CommandPrefix  = (string?)value; break;
                    case nameof(ArgumentPrefix)        : attribute.ArgumentPrefix = (string?)value; break;
                    case nameof(OptionPrefix)          : attribute.OptionPrefix   = (string?)value; break;
                    case nameof(CommandSuffix)         : attribute.CommandSuffix  = (string?)value; break;
                    case nameof(ArgumentSuffix)        : attribute.ArgumentSuffix = (string?)value; break;
                    case nameof(OptionSuffix)          : attribute.OptionSuffix   = (string?)value; break;
                    case var key: throw new SwitchExpressionException(key);
                }
            }

            return attribute;
        }
    }
}

#endif // DOCOPTNET_INTERNAL
