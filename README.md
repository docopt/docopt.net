# Templates for [docopt.net]

Templates for use with `dotnet new` to create .NET console applications that
use [docopt.net] for command-line parsing support.

## Installation

To install:

    dotnet new --install docopt.net.templates

To uninstall, run:

    dotnet new --uninstall docopt.net.templates

## Usage

Once installed, a new console application can be created the following
command:

    dotnet new docopt-console -o ConsoleApp

## Packaging

To package all templates for distribution, run:

    dotnet pack

[docopt.net]: https://docopt.github.io/docopt.net/
