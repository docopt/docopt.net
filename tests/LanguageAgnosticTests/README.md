Language Agnostic Tests
=======================

Compile the **Testee** project then run the following commands:

    dotnet build tests/Testee
    cd tests/LanguageAgnosticTests
    python language_agnostic_tester.py ../Testee/bin/Debug/net6.0/Testee

On Windows, use backslash (`\`) as the path separator:

    dotnet build tests\Testee
    cd tests\LanguageAgnosticTests
    python language_agnostic_tester.py ..\Testee\bin\Debug\net6.0\Testee.exe
