# Integration Tests

The integration tests project exercises the full integration of the source
generator into the C# compiler via the **docopt.net** NuGet package.

Depending on the host platform and shell, execute either `run.cmd` (Windows),
`run.sh` (BASH) or `run.ps1` (PowerShell Core) to exercise the tests. The script
will build the package (unless the `-NoPack` argument is supplied), build the
tests project in the release configuration and then exercise the tests.
