# Source Generator Design

C# 9 introduced [C# source generators] that enables a _source generator_
implementation to _add_ C# constructs and code to a compilation at compile-time.
Since [docopt] is a formal description language for expressing a command-line
interface (CLI) as part of a program's help text, a source generator can take
that description and produce code to parse arguments per the specification. This
is a design and concept document for such a source generator implementation in
**docopt.net**.

## Terminology

The word _docopt_ shall be used to mean the command-line description language
described at [docopt.org][docopt].

The words _docopt source_ or _docopt text_ shall be used to mean text conforming
to the _docopt_ lanaguge.

The word _argument_ shall generally be used to refer to a command, a positional
argument or an option.

The words _positional argument_ shall be used to mean an `<argument>` as defined
in the docopt specification.

## Requirements

The source generator should:

- be shipped as a NuGet package for installation and use during development.

- be very simple to use, requiring as little setup and ceremony as possible.

- not require understanding of any new concepts from the developer (at least for
  initial use) beyond the understanding that a class holds the parsed program
  arguments and a method can be used to process a string array of arguments into
  an instance of that class.

- allow the developer to incrementally (as needed) control various and
  reasonable, but not all, aspects of the generated code.

- use reasonable defaults and conventions that yield the least element of
  surprise in the absence of any customization by the developer.

- generate strong-typed code and support the basic types needed to represent
  values of command, positional arguments and options.

- generate efficient code.

- generate code that does not require any type information at run-time
  (reflection) for any decisions.

- statically validate models at compile-time to avoid run-time errors.

- support text localization at run-time; it does not need to support it
  directly, but should not hinder the option for a developer.

- provide useful error information whenever possible instead of throwing
  unhandled exceptions.

- support generating code for multiple docopt sources.

- not require any additional NuGet package to be installed, not even
  [docopt.net].

## Overview

The source generator uses some docopt text to drive the generated code. That
text can come from two places:

1. A plain text file with the extension `.docopt.txt` and which is supplied by
   the C# compiler as an _additional file_.

2. A C# string constant in the program.

The first case is expected to be the most common. The second case is supported
for simple CLI applications where the entire program is simple enough that the
developer may wish to keep everything in a single C# source. In contrast, the
first case will always require at least two files: a plain text file that is the
docopt source and a C# source file with the code for the CLI application.

Given a docopt source, the source generator will generate a C# class that
contains a property for each command, positional argument and option that
appears in the docopt source. It will also contain a static method taking a
sequence of strings representing arguments to be parsed and returning an
instance of the class with the arguments parsed and loaded into the various
properties.

Suppose a developer adds a file called `Program.docopt.txt` to a C# project file
named `NavalFate.csproj` with the following content:

    Naval Fate.

    Usage:
      naval_fate.exe ship new <name>...
      naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
      naval_fate.exe ship shoot <x> <y>
      naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
      naval_fate.exe (-h | --help)
      naval_fate.exe --version

    Options:
      -h --help     Show this screen.
      --version     Show version.
      --speed=<kn>  Speed in knots [default: 10].
      --moored      Moored (anchored) mine.
      --drifting    Drifting mine.

The source generator will produce a class as follows (some implementation
details have been skipped for brevity and comments added for annotation but may
not be part of the generated code):

```c#
namespace NavalFate
{
    // The "arguments class".

    partial class ProgramArguments :
        IEnumerable<KeyValuePair<string, object?>>
    {
        // The default constructor that initializes defaults.

        public ProgramArguments()
        {
            ArgName = StringList.Empty;
            OptSpeed = "10";
        }

        // The "help constant" with the docopt source/text.

        public const Help = @"Naval Fate.

Usage:
  naval_fate.exe ship new <name>...
  naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
  naval_fate.exe ship shoot <x> <y>
  naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
  naval_fate.exe (-h | --help)
  naval_fate.exe --version

Options:
  -h --help     Show this screen.
  --version     Show version.
  --speed=<kn>  Speed in knots [default: 10].
  --moored      Moored (anchored) mine.
  --drifting    Drifting mine.
";

        // The "usage constant" with the usage section from above.

        public const Usage = @"Usage:
  naval_fate.exe ship new <name>...
  naval_fate.exe ship <name> move <x> <y> [--speed=<kn>]
  naval_fate.exe ship shoot <x> <y>
  naval_fate.exe mine (set|remove) <x> <y> [--moored | --drifting]
  naval_fate.exe (-h | --help)
  naval_fate.exe --version
";

        // The "argument properties" where each represents:
        // - a command
        // - a positional argument
        // - or an option

        public bool CmdShip { get; private set; }
        public bool CmdNew { get; private set; }
        public StringList ArgName { get; private set; }
        public bool CmdMove { get; private set; }
        public string? ArgX { get; private set; }
        public string? ArgY { get; private set; }
        public string OptSpeed { get; private set; }
        public bool CmdShoot { get; private set; }
        public bool CmdMine { get; private set; }
        public bool CmdSet { get; private set; }
        public bool CmdRemove { get; private set; }
        public bool OptMoored { get; private set; }
        public bool OptDrifting { get; private set; }
        public bool OptHelp { get; private set; }
        public bool OptVersion { get; private set; }

        // The "application method".

        public static ProgramArguments Parse(IEnumerable<string> args) {
            // ...
        }

        // The "iterator implementation".

        IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            yield return KeyValuePair.Create("ship", (object?)CmdShip);
            yield return KeyValuePair.Create("new", (object?)CmdNew);
            yield return KeyValuePair.Create("<name>", (object?)ArgName);
            yield return KeyValuePair.Create("move", (object?)CmdMove);
            yield return KeyValuePair.Create("<x>", (object?)ArgX);
            yield return KeyValuePair.Create("<y>", (object?)ArgY);
            yield return KeyValuePair.Create("--speed", (object?)OptSpeed);
            yield return KeyValuePair.Create("shoot", (object?)CmdShoot);
            yield return KeyValuePair.Create("mine", (object?)CmdMine);
            yield return KeyValuePair.Create("set", (object?)CmdSet);
            yield return KeyValuePair.Create("remove", (object?)CmdRemove);
            yield return KeyValuePair.Create("--moored", (object?)OptMoored);
            yield return KeyValuePair.Create("--drifting", (object?)OptDrifting);
            yield return KeyValuePair.Create("--help", (object?)OptHelp);
            yield return KeyValuePair.Create("--version", (object?)OptVersion);
        }

        IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
```

The class will:

- use the root namespace (`NavalFate` above) of the project, if any.

- use the `partial` modifier so a developer can extend its definition.

- implement a parameter-less (default) constructor to initialise options and
  positional arguments with defaults.

- have a `const` string member named `Help` with the original docopt text.

- have a `const` string member named `Usage` with just the _usage section_ from
  `Help`.

- have a read-only property for each command, positional argument and option.

- have a static method named `Parse` that will take a sequence of strings
  representing arguments to be parsed and return an instance of the class.

- implement `IEnumerable<KeyValuePair<string, object?>>` where the
  implementation will enumerate all arguments as key-value pairs.

## `DocoptArgumentsAttribute`

A `[DocoptArguments]` attribute can be applied to an [arguments class] and
serves two main purposes:

1. Marks a class _part_ written by the developer as an [arguments class].

2. Control customizations offered by the source generator.

The attribute is required in the second case.

Example:

```c#
namespace NavalFate
{
    [DocoptArguments]
    partial class ProgramArguments { }
}
```

The various customizations offered through the attribute are described in the
remaining sections of this document.

## [Help Constant]

The generated [arguments class] will offer a public `const` string member that
contains the entire docopt source. By default, this member will be named `Help`.
If there is a conflict with another identically named member added by the
developer to a part of the [arguments class] then the C# compiler will emit a
[CS0102] error. The source generator does not need to do any detection of
conflicts.

  The developer can specify a different name for the help constant as follows:

```c#
namespace NavalFate
{
    [Docopt(HelpConstName = "HelpText")]
    partial class ProgramArguments { }
}
```

When generated, the purpose of the help constant is to allow the developer
access to the original docopt source for displaying help to the user.

Instead of providing the docopt source in an external plain text file, the
developer can embed it by providing the help constant:

```c#
namespace NavalFate
{
    [DocoptArguments]
    partial class ProgramArguments
    {
        const string Help = "...";
    }
}
```

- The containing class must be decorated with `[DocoptArguments]`.
- The field must be a `const`.
- The `const` must be typed as a string.
- The name must be `Help` unless a different name is specified via
  `[DocoptArguments]`.
- The constant field can have any desired accessibility.

> Questions:
> - What will localization look like for help?
> - What if the developer wants to replace placeholders in the help text with
>   run-time values?

## Usage Constant

The generated [arguments class] will offer a public `const` string member that
contains the usage section from the docopt source. By default, this member will
be named `Usage`. If there is a conflict with another identically named member
added by the developer to a part of the [arguments class] then the C# compiler
will emit a [CS0102] error. The source generator does not need to do any
detection of conflicts.

The developer can specify a different name for the constant as follows:

```c#
namespace NavalFate
{
    [Docopt(UsageConstName = "UsageText")]
    partial class ProgramArguments { }
}
```

The accessibility of the constant can be changed as well:

```c#
namespace NavalFate
{
    [Docopt(UsageConstAccessibility = FieldAttributes.Private)]
    partial class ProgramArguments { }
}
```

The source generator will emit an error if the specified [`FieldAttributes`][fattr] is _not_ one of the following:

- `Private` (`private`)
- `Assembly` (`internal`)
- `Family` (`protected`)
- `FamORAssem` (`protected internal`)
- `Public` (`public`)

The usage constant can be used to display brief help when, for example,
command-line arguments do not match any of the usages listed in the docopt
source.

## [Arguments Class]

The arguments class is the principal output of the source generator for a docopt
source. An instance represents the parsed arguments.

The generation of an arguments class is triggered in one of two ways:

- **External**: The docopt text is in a plain text file that is part of the
  project.

- **Inline**: The docopt text is in a string constant (see [help constant]) that
  is a member of an arguments class part.

In the external case, an arguments class is generated for each plain text file
in the project whose name ends in `.docopt.txt`. The file name can end
differently if the developer specifies an `<AdditionalFiles>` element with an
attribute `SourceItemType` set to the value `docopt` (case-insensitive):

```xml
<ItemGroup>
  <AdditionalFiles Include="NavalFateHelp.txt"
                   SourceItemType="docopt">
</ItemGroup>
```

In the inline case, an arguments class is also generated for any class in the
project decorated with the `[DocoptArguments]` attribute _and_ having a [help
constant] defined by the developer:

```c#
namespace NavalFate
{
    [DocoptArguments]
    partial class ProgramArguments
    {
        const string Help = "...";
    }
}
```

If the [help constant] is missing and there is no associated external file, the
source generator will emit a diagnostics error.

The above two cases are mutually exclusive. The source generator will emit a
diagnostics error if both are used.

When the docopt source is defined in an external file, an arguments class part defined by the developer can be _explicitly linked_ or _implicitly linked_.

Explicit linking enables the developer to choose the namespace and the name of
the arguments class by creating a part and _explicitly linking_ it to the file
as show here:

```c#
namespace NavalFate
{
    [DocoptArguments(HelpFile = @"project/relative/path/to/help.txt")]
    partial class ProgramArguments
    {
    }
}
```

The source generator treats the `HelpFile` path components as being
_case-sensitive_, but uses `/` as the normalized path separator for platform
portability. A diagnostics error is issued if there is no such _additional file_
in the project. If the file is linked (e.g. `<None
Include="../Program.docopt.txt" Link="Program.docopt.txt" />`) into the project
then the path is based on the value of the `Link` attribute.

In the absence of any _explicit link_, the source generator will choose the
namespace and the name for the arguments class part it generates. The class name
will use the same name as the file, minus the `.docopt.txt` ending, plus the
`Arguments` suffix. The namespace will come from the project's root namespace
plus the path transformed into a sub-namespace. Suppose a project has a root
namespace of `Foo` and contains the file at the relative path
`Bar/Baz.docopt.txt`. The source generator will produce an arguments class as
follows:

```c#
namespace Foo.Bar
{
    partial class BazArguments
    {
        // generated code...
    }
}
```

A developer can augment/customize the generated arguments class part by defining
another part in the project under the same namespace and using the same name
(note that the developer part is required to use the `[DocoptArguments]`
attribute):

```c#
namespace Foo.Bar
{
    [DocoptArguments] // required
    partial class BazArguments
    {
        // developer code...
    }
}
```

The above is considered by the source generator to be _implicitly linked_.

### Extension

The generated [arguments class] will bear the `partial` modifier so a developer
can extend its definition by introducing another part in the project.

## [Argument Properties]

The source generator generates a public read-only property for every command,
positional argument and option in the docopt source.

The source generator will skip the generation a property if the developer
defines one in another part of the [arguments class]:

```c#
namespace NavalFate
{
    partial class ProgramArguments
    {
        [DocoptArgument("--speed")]
        public string Speed { get; private set; }
    }
}
```

The developer has overall control over the property definition as long as the
property definition:

- does not use the `static` modifier.

- has the `[DocoptArgument]` attribute applied and identifies the corresponding
  argument in the docopt source.

The argument identified by the `[DocoptArgument]` attribute must use docopt
conventions, i.e. `command` for a command, `<argument>` for a positional
argument and `-o` or `--option` for an option. If an option has a long name and
a short name, then either of the two may be specified (but not both).

The source generator emits a diagnotics error if the argument does not exist or
conflicts with another another property (e.g. another property is defined for the same argument).

### Naming

The source generator names an argument property based on a transformation of the argument name in the docopt source that follows the algorithm:

1. For short options, the leading hypen (`-`) is removed.

2. For long options, the leading two hypens (`--`) are removed.

3. For position arguments, the `<` is remove at the head and the `>` is removed
   at the tail.

4. The remaining characters are changed to Pascal case by capitalizing the first
   character and every character following a hypen (`-`). For example, `foo-bar`
   becomes `Foo-Bar`.

5. All hyphen (`-`) occurrences are removed.

6. A prefix is added to the name depending on the argument type:

   - `Cmd` for a command.
   - `Opt` for an option.
   - `Arg` for a positional argument

If the resulting name is not a valid C# identifier then the source generator
lets the C# compiler produce an error.

The `Cmd`, `Opt`, `Arg` prefixes can be changed by the developer:

```c#
namespace NavalFate
{
    [DocoptArguments(CommandPrefix  = "Command",
                     ArgumentPrefix = "Argument",
                     OptionPrefix   = "Option")]
    partial class ProgramArguments
    {        
    }
}
```

Likewise, the developer can also designate suffixes for each:

```c#
namespace NavalFate
{
    [DocoptArguments(CommandSuffix  = "Command",
                     ArgumentSuffix = "Argument",
                     OptionSuffix   = "Option")]
    partial class ProgramArguments
    {        
    }
}
```

If a suffix is supplied, then the source generator will omit the default prefix
unless one is explicitly defined by the developer. The suppression of prefixes
in the following case therefore is redundant:

```c#
namespace NavalFate
{
    [DocoptArguments(
        CommandPrefix  = "", CommandSuffix  = "Command",
        ArgumentPrefix = "", ArgumentSuffix = "Argument",
        OptionPrefix   = "", OptionSuffix   = "Option")]
    partial class ProgramArguments
    {        
    }
}
```

### Types

The source generator selects an appropriate type for an argument property
based on the following rules and the type of the argument (command, positional
or option):

- Command:

  - A command property will have the type `bool`.

  - If the command can appear more than once, for example it is specified as
    `command...` in the docopt source, then the command property will have the
    type `int` and its value will reflect the actual repetition count.

- Positional Argument:

  - A command property will have the type `string`.

  - If the positional argument can appear more than once, for example it is
    specified as `<argument>...` in the docopt source, then the positional
    argument property will have the type `StringList` and its value will reflect
    the actual parsed values.

- Option:

  - A property for an option without any value (e.g. `--option`) will have the
    type `bool`.

  - A property for an option with a value (e.g. `--option=value`) will have the
    type `string`.

  - If an option without any value can appear more than once, for
    example it is specified as `--option...` in the docopt source, then the
    option property will have the type `int` and its value will reflect the
    actual repetition count.

  - If an option with a value can appear more than once, for example it is
    specified as `--option=<value>...` in the docopt source, then the option
    property will have the type `StringList` and its value will reflect the
    actual parsed values.

### Type Conversions

If the developer wants to use different types for [argument properties] then a
conversion is necessary. The developer must:

- define the argument property with the desired type and decorate it with the
  `[DocoptArgument]` attribute.

- supply a static method named after the property but with the suffix `Parse`.
  The method must take a single argument based on the appropriate type (`bool`,
  `int`, `string`, `StringList` or  `object`) and return a value of the same
  type as the property.

The source generator scans the static [arguments class] methods for conversion
methods. A conversion method for a property based on the above rules is only
selected if the property does not have the expected type. Once selected, the
signature is validated and an error diagnostics is emitted if it is
non-conforming.

The generated code will call the conversion method and use the result to assign
a value to the property.

A simple conversion example:

```c#
namespace NavalFate
{
    [DocoptArguments]
    partial class ProgramArguments
    {
        [DocoptArgument("<url>")]
        public Uri Url { get; private set; }

        static Uri ParseUrl(string arg) => new Uri(arg);
    }
}
```

If several [argument properties] need to share the same coversion method then
the method can be named using the `Parser` property of the `[DocoptArgument]`
attribute as shown here:

```c#
namespace NavalFate
{
    [DocoptArguments]
    partial class ProgramArguments
    {
        [DocoptArgument("<source>", Parser = nameof(ParseUrl))]
        public Uri SourceUrl { get; private set; }

        [DocoptArgument("<target>", Parser = nameof(ParseUrl))]
        public Uri TargetUrl { get; private set; }

        static Uri ParseUrl(string arg) => new Uri(arg);
    }
}
```

> Questions:
> - Is `IFormatProvider` support needed?
> - What about supporting parsing options?

## [Parse Method]

The [parse method] is the main workhorse of the code generated by the source
generator. It accepts a single argument that is a sequence of strings, parses it
per the usage rules in the docopt source and returns an instance of the
[arguments class] with properties loaded.

```c#
public static Arguments Parse(IEnumerable<string> args)
{    
    // generated code...
}
```

If the input argument do not conform to the usage, the implementation throws an
instance of `DocoptInputErrorException`.

> Question: Should the signatrue be based on an attempt to parse?
>
> ```c#
> public static bool Parse(IEnumerable<string> args,
>                          [NotNullWhen(true)]out Arguments? result);
> ```

(TBC)

### Error Handling

(TBD)

### Exit Handing

(TBD)

## Iterator Implementation

The source generator provides an implementation of
`IEnumerable<KeyValuePair<string, object?>>` for an [arguments class] where:

  - its members are implemented explicitly.

  - the implementation _lazily_ enumerate all arguments as key-value pairs,
    where the key is the argument name as specified in the docopt source and the
    value will be the value of the corresponding property converted to a
    nullable `object`.

If an option has a long and short form (e.g. `-o` and `--option`), then the long
form is returned in the key.

## Nullable Context

The source generator respects the nullable context of the project.

## Diagnostic Identifier

All diagnostics produced by the generator use the identifier format `DCPT####` where `####` is a 4-digit number (left-padded with zeros) starting
with 1, e.g. `DCPT0001`.


  [docopt]: http://docopt.org/
  [docopt.net]: https://www.nuget.org/packages/docopt.net/
  [arguments class]: #arguments-class
  [argument properties]: #argument-properties
  [help constant]: #help-constant
  [parse method]: #parse-method
  [C# source generators]: https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/
  [CS0102]: https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0102
  [fattr]: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.fieldattributes
