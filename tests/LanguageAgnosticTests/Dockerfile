FROM mcr.microsoft.com/dotnet/sdk:7.0.101-alpine3.16-amd64 AS build

WORKDIR /project

COPY Directory.* .
COPY src/DocoptNet src/DocoptNet
COPY tests/LanguageAgnosticTests tests/LanguageAgnosticTests

RUN dotnet build tests/LanguageAgnosticTests

FROM mcr.microsoft.com/dotnet/runtime:7.0.1-alpine3.16-amd64

WORKDIR /tests

COPY --from=build /project/tests/LanguageAgnosticTests/bin/Debug/net7.0/ bin
COPY --from=build /project/tests/LanguageAgnosticTests/*.py .
COPY --from=build /project/tests/LanguageAgnosticTests/*.docopt .

RUN apk add --no-cache python3

CMD [ "python3" , "language_agnostic_tester.py", "bin/Testee" ]
