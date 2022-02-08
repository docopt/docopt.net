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
    GitArguments.CreateParser().WithVersion("git version 1.7.4.4").Run(args, true, null, null, 1, args =>
    {
        Console.WriteLine("global args:");
        foreach (var (name, value) in args)
            Console.WriteLine($"{name} = {value}");

        Console.WriteLine("command args:");

        int Run<T>(IParserWithHelpSupport<T> parser)
            where T : IEnumerable<KeyValuePair<string, object?>>
        {
            var argv = new[] { args.ArgCommand! }.Concat(args.ArgArgs).ToArray();

            return parser.Run(argv, args =>
            {
                foreach (var (name, value) in args)
                    Console.WriteLine($"{name} = {value}");
                return 0;
            });
        }

        return args.ArgCommand switch
        {
            "add"       => Run(GitAddArguments.CreateParser()),
            "branch"    => Run(GitBranchArguments.CreateParser()),
            "checkout"  => Run(GitCheckoutArguments.CreateParser()),
            "clone"     => Run(GitCloneArguments.CreateParser()),
            "commit"    => Run(GitCommitArguments.CreateParser()),
            "push"      => Run(GitPushArguments.CreateParser()),
            "remote"    => Run(GitRemoteArguments.CreateParser()),
            var command => throw new ApplicationException($"{command} is not a Git command. See 'git help'.")
        };
    });
