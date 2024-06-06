Language Agnostic Tests
=======================

Compile the **Testee** project then run the following commands:

    cd tests/LanguageAgnosticTests
    dotnet build
    python language_agnostic_tester.py bin/Debug/net8.0/Testee

On Windows, use the following commands:

    dotnet build tests\LanguageAgnosticTests
    cd tests\LanguageAgnosticTests
    python language_agnostic_tester.py bin\Debug\net8.0\Testee.exe

Alternatively, build and run the following Docker image (assuming the current
working directory is the same as the directory containing this file):

    docker build -t docopt-net-lang-tests -f Dockerfile ../..
    docker run --rm docopt-net-lang-tests
