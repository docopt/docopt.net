#nullable enable
#r "nuget: docopt.net, 0.6.1.11"
#r "nuget: CliWrap, 3.3.3"
#r "nuget: morelinq, 3.3.0"

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using CliWrap;
using CliWrap.Buffered;
using DocoptNet;
using static MoreLinq.Extensions.SplitExtension;

const string doc = @"Usage:
  sgss inspect [-wi] [--no-gui]
  sgss accept
  sgss show [-v]
  sgss clean [-f]
  sgss -h | --help

Options:
  -h, --help     Show this screen.
  -v, --verbose  Show verbose output.
  -f, --force    Force the operation.
  -w, --wait     Wait for Git GUI to exit.
  --no-gui       Don't launch Git GUI.
  -i             Interactive; launches Git GUI, waits then cleans up.
";

var args = new Docopt().Apply(doc, Args, help: true, exit: true);
var force = args["--force"].IsFalse;

var git = Cli.Wrap("git");

try
{
    if (!await git.WithArguments("rev-parse", "--is-inside-work-tree")
                  .ExecuteBufferedAsync(r => "true" == r.StandardOutput.Trim()))
    {
        return PrintError("The current working directory is not a Git work tree.");
    }

    var workTreePath = await git.WithArguments("rev-parse", "--show-toplevel")
                                .ExecuteBufferedAsync(r => r.StandardOutput.Trim());

    IEnumerable<string> EnumerateTestDiffFiles() =>
        Directory.EnumerateFiles(Path.Combine(workTreePath, "tests"), ".testdiff", SearchOption.AllDirectories);

    if (args["show"].IsTrue)
    {
        var verbose = args["--verbose"].IsTrue;

        foreach (var path in EnumerateTestDiffFiles())
        {
            Console.WriteLine(path);
            if (verbose)
            {
                foreach (var line in File.ReadLines(path))
                {
                    Console.Write("  ");
                    Console.WriteLine(line);
                }
            }
        }
    }
    else if (args["clean"].IsTrue)
    {
        var branchName = await GetBranchNameFromHead();
        var tempBranchName = $"sgss/{branchName}";

        if (!await DoesBranchExist(tempBranchName))
            return PrintError($"Branch '{tempBranchName}' not found. Did you forget to run inspect?");

        var soughtBranchLine = $"branch refs/heads/{tempBranchName}";

        var workTreeListLines =
            await git.WithArguments("worktree", "list", "--porcelain")
                     .ExecuteBufferedAsync(r => r.StandardOutput.SplitIntoLines());

        var workTreeLine =
            workTreeListLines
                .Split(string.IsNullOrWhiteSpace)
                .Where(r => r.Any(s => s == soughtBranchLine))
                .Select(r => r.FirstOrDefault(s => s.StartsWith("worktree ", StringComparison.Ordinal)))
                .FirstOrDefault();

        if (workTreeLine is null)
            return PrintError($"Work tree not found for branch: {tempBranchName}");

        var tempWorkTreePath = workTreeLine.Split(' ', 2)[1];

        if (force && await IsWorkTreeDirty(tempWorkTreePath))
            return PrintError($"'{tempWorkTreePath}' contains modified or untracked files; use --force to delete it.");

        _ = await git.WithArguments("worktree", "remove", "--force", tempWorkTreePath).ExecuteAsync();
        _ = await git.WithArguments("branch", "-d", tempBranchName).ExecuteAsync();
    }
    else if (args["inspect"].IsTrue)
    {
        using var testDiffFilePath = EnumerateTestDiffFiles().GetEnumerator();
        if (!testDiffFilePath.MoveNext())
        {
            Console.Error.WriteLine("No test differences to inspect.");
            return 0;
        }

        var branchName = await GetBranchNameFromHead();
        var tempBranchName = $"sgss/{branchName}";

        if (await DoesBranchExist(tempBranchName))
            Console.Error.WriteLine($"A branch named '{tempBranchName}' already exists.");
        else
            _ = await git.WithArguments($"branch {tempBranchName}").ExecuteAsync();

        var tempWorkTreePath = Path.GetFullPath($"{workTreePath}/../{Path.GetFileName(workTreePath)}.sgss/{branchName}");

        Console.WriteLine($"{tempWorkTreePath} [{tempBranchName}]");

        _ = await git.WithArguments("worktree", "add", tempWorkTreePath, tempBranchName).ExecuteAsync();

        var tree = await git.WithArguments("write-tree").ExecuteBufferedAsync(r => r.StandardOutput.Trim());
        _ = await git.WithArguments("read-tree", tree).WithWorkingDirectory(tempWorkTreePath).ExecuteAsync();
        _ = await git.WithArguments("checkout", "--", ".").WithWorkingDirectory(tempWorkTreePath).ExecuteAsync();

        do
        {
            ApplyTestDiff(workTreePath, tempWorkTreePath, File.ReadLines(testDiffFilePath.Current), Console.Out);
        }
        while (testDiffFilePath.MoveNext());

        var isInteractive = args["-i"].IsTrue;
        var shouldShowGui = args["--no-gui"].IsFalse;

        if (isInteractive && !shouldShowGui)
            return PrintError("The options -i and --no-gui are incompatible.");

        if (shouldShowGui)
        {
            var gitGuiTask = git.WithArguments("gui")
                                .WithWorkingDirectory(tempWorkTreePath)
                                .WithValidation(CommandResultValidation.None)
                                .ExecuteAsync();

            if (isInteractive || args["--wait"].IsTrue)
            {
                Console.Error.WriteLine("Waiting for Git GUI to exit...");
                _ = await gitGuiTask;
            }

            if (isInteractive)
            {
                _ = await git.WithArguments("worktree", "remove", "--force", tempWorkTreePath).ExecuteAsync();
                _ = await git.WithArguments("branch", "-d", tempBranchName).ExecuteAsync();
            }
        }
    }
    else if (args["accept"].IsTrue)
    {
        foreach (var path in EnumerateTestDiffFiles())
            ApplyTestDiff(workTreePath, workTreePath, File.ReadLines(path), Console.Out);
    }
    else
    {
        throw new NotImplementedException();
    }
}
catch (CliWrap.Exceptions.CommandExecutionException ex)
{
    return PrintError(ex.Message, ex.ExitCode);
}
catch (NotSupportedException ex)
{
    return PrintError(ex.Message, exitCode: 1);
}

static void ApplyTestDiff(string sourceDirPath, string targetDirPath, IEnumerable<string> lines,
                          TextWriter writer)
{
    var diffs =
        from line in lines
        select line.Trim() into line
        where line.Length > 0 && line[0] != '#'
        select line.Split('\t') into tokens
        select new
        {
            Status = tokens[0][0],
            Target = Path.Combine(targetDirPath, tokens[1]),
            Source = tokens.Length > 2 ? Path.Combine(sourceDirPath, tokens[2]) : null,
        };

    foreach (var diff in diffs)
    {
        switch (diff.Status)
        {
            case 'D':
                writer?.WriteLine($"{diff.Status} {diff.Target}");
                File.Delete(diff.Target);
                break;
            case 'M':
                writer?.WriteLine($"{diff.Status} {diff.Target}");
                File.Copy(diff.Source, diff.Target);
                break;
            case '?':
                writer?.WriteLine($"{diff.Status} {diff.Target}");
                Directory.CreateDirectory(Path.GetDirectoryName(diff.Target)!);
                File.Copy(diff.Source, diff.Target);
                break;
        }
    }
}

async Task<bool> IsWorkTreeDirty(string path) =>
    await git.WithArguments("status", "--porcelain")
             .WithWorkingDirectory(path)
             .ExecuteBufferedAsync(r => r.StandardOutput.Length > 0);

async Task<string> GetBranchNameFromHead() =>
    await git.WithArguments("rev-parse", "--abbrev-ref", "HEAD")
             .ExecuteBufferedAsync(r => r.StandardOutput.Trim()) switch
             {
                 "HEAD" => throw new NotSupportedException($"HEAD is in detached state, which is currently not supported."),
                 var name => name,
             };

async Task<bool> DoesBranchExist(string name) =>
    await git.WithArguments("show-ref", "-q", "--verify", $"refs/heads/{name}")
             .WithValidation(CommandResultValidation.None)
             .ExecuteBufferedAsync() is { ExitCode: 0 };

static int PrintError(string message, int? exitCode = null)
{
    Console.Error.WriteLine(message);
    return exitCode ?? 1;
}

public static IEnumerable<string> SplitIntoLines(this string str)
{
    using var reader = new StringReader(str);
    while (reader.ReadLine() is { } line)
        yield return line;
}

public static Command WithArguments(this Command command, params string[] args) =>
    command.WithArguments(args);

public static Task<T> ExecuteBufferedAsync<T>(this Command command,
                                              Func<BufferedCommandResult, T> selector) =>
    command.ExecuteBufferedAsync(CancellationToken.None, selector);

public static async Task<T>
    ExecuteBufferedAsync<T>(this Command command,
                            CancellationToken cancellationToken,
                            Func<BufferedCommandResult, T> selector) =>
    selector(await command.ExecuteBufferedAsync().ConfigureAwait(false));
