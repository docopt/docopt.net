using System;
using DocoptNet;
using ArgumentsExample;

return ProgramArguments.Parse(args).Run(args =>
{
    foreach (var (name, value) in args)
        Console.WriteLine($"{name} = {value}");

    Console.WriteLine($@"{{
    Help       = {args.OptHelp},
    V          = {args.OptV},
    Q          = {args.OptQ},
    R          = {args.OptR},
    Left       = {args.OptLeft},
    Right      = {args.OptRight},
    Correction = {args.ArgCorrection},
    File       = [{string.Join(", ", args.ArgFile)}],
}}");

    return 0;
});
