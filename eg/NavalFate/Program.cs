using System;
using DocoptNet;

const string usage = @"Naval Fate.

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

try
{
    var arguments = new Docopt().Apply(usage, args, version: "Naval Fate 2.0", exit: true)!;
    foreach (var argument in arguments)
    {
        Console.WriteLine("{0} = {1}", argument.Key, argument.Value);
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
