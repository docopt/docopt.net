# **docopt.net** is a .NET implementation of [docopt]

**docopt.net** is a parser for command-line arguments. It automatically derives
the parsing logic from the help text of a program containing its command-line
usage in [docopt] format. [docopt] is the formalization of conventions that have
been used for decades in help messages and man pages for describing a program's
interface. Below is an example of such a help text containing the usage for a
hypothetical program called Naval Fate:

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

## Usage

The following C# example shows one basic way to use **docopt.net**:

```c#
#nullable enable

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

var arguments = new Docopt().Apply(usage, args, version: "Naval Fate 2.0", exit: true)!;
foreach (var (key, value) in arguments)
    Console.WriteLine("{0} = {1}", key, value);
```

See the [documentation] for more examples and uses of the **docopt.net** API.

## Documentation

The documentation can be found online at <https://docopt.github.io/docopt.net/>.

## Installation

Install [the package][nupkg] in a .NET project using:

    dotnet add package docopt.net

## Copyright and License

- &copy; 2012-2014 Vladimir Keleshev <vladimir@keleshev.com>
- &copy; 2013 Dinh Doan Van Bien <dinh@doanvanbien.com>
- &copy; 2021 Atif Aziz
- Portions &copy; .NET Foundation and Contributors
- Portions &copy; West Wind Technologies, 2008

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

[docopt]: http://docopt.org/
[nupkg]: https://www.nuget.org/packages/docopt.net
[documentation]: https://docopt.github.io/docopt.net/
