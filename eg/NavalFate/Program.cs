using System;
using System.Collections.Generic;
using DocoptNet;

const string help = @"Naval Fate.

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

var argsParser = Docopt.CreateParser(help).WithVersion("Naval Fate 2.0");

static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
static int ShowVersion(string version) { Console.WriteLine(version); return 0; }
static int OnError(string usage) { Console.WriteLine(usage); return 1; }

static int Run(IDictionary<string, ArgValue> arguments)
{
    foreach (var (key, value) in arguments)
        Console.WriteLine("{0} = {1}", key, value);
    return 0;
}

#if !USE_PATTERN_MATCHING

return argsParser.Parse(args)
                 .Match(Run,
                        result => ShowHelp(result.Help),
                        result => ShowVersion(result.Version),
                        result => OnError(result.Usage));

#else // Alternatively, using pattern-matching on the result:

return argsParser.Parse(args) switch
{
    IArgumentsResult<IDictionary<string, ArgValue>> { Arguments: var arguments } => Run(arguments),
    IHelpResult => ShowHelp(help),
    IVersionResult { Version: var version } => ShowVersion(version),
    IInputErrorResult { Usage: var usage } => OnError(usage),
    var result => throw new System.Runtime.CompilerServices.SwitchExpressionException(result)
};

#endif
