Language Agnostic Tests
=======================

Compile the **Testee** project then run the following commands:

    dotnet build src/Testee
    cd src/LanguageAgnosticTests
    python language_agnostic_tester.py ../Testee/bin/Debug/net6.0/Testee.exe
