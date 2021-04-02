Language Agnostic Tests
=======================

Method 1: testee app
--------------------
1. Compile the Testee project
2. Run the following commands
	cd src\LanguageAgnosticTests
    python language_agnostic_tester.py ..\Testee\bin\Debug\net45\Testee.exe

Method 2: generated unit tests
------------------------------

1. Generate the tests
	cd src\LanguageAgnosticTests
    generate_tests >..\DocoptNet.Tests\LanguageAgnosticTests.generated.cs
2. Run all the unit tests
