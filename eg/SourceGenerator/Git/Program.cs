#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DocoptNet;
using Git;

try
{
    return Run(args);
}
catch (DocoptExitException e)
{
    Console.WriteLine(e.Message);
    return e.ErrorCode;
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
    return 0xbd;
}

static int Run(string[] args) =>
    GitArguments.Parse(args, Docopt.ParseFlags.OptionsFirst, "git version 1.7.4.4").Run(args =>
    {
        Console.WriteLine("global args:");
        foreach (var (name, value) in args)
            Console.WriteLine($"{name} = {value}");

        Console.WriteLine("command args:");

        int Run<T>(Func<IEnumerable<string>, IParser<T>.IResult> f)
            where T : IEnumerable<KeyValuePair<string, object?>>
        {
            var argv = new[] { args.ArgCommand! }.Concat(args.ArgArgs).ToArray();

            return f(argv).Run(args =>
            {
                foreach (var (name, value) in args)
                    Console.WriteLine($"{name} = {value}");
                return 0;
            });
        }

        return args.ArgCommand switch
        {
            "add"       => Run(args => GitAddArguments.Parse(args)),
            "branch"    => Run(args => GitBranchArguments.Parse(args)),
            "checkout"  => Run(args => GitCheckoutArguments.Parse(args)),
            "clone"     => Run(args => GitCloneArguments.Parse(args)),
            "commit"    => Run(args => GitCommitArguments.Parse(args)),
            "push"      => Run(args => GitPushArguments.Parse(args)),
            "remote"    => Run(args => GitRemoteArguments.Parse(args)),
            var command => throw new ApplicationException($"{command} is not a Git command. See 'git help'.")
        };
    });
