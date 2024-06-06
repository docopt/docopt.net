FROM mcr.microsoft.com/dotnet/sdk:8.0.300-alpine3.19-amd64 AS build

WORKDIR /project

COPY Directory.* .
COPY src/DocoptNet src/DocoptNet
COPY tests/LanguageAgnosticTests tests/LanguageAgnosticTests

RUN dotnet build tests/LanguageAgnosticTests

FROM mcr.microsoft.com/dotnet/runtime:8.0.6-alpine3.19-amd64

WORKDIR /tests

COPY --from=build /project/tests/LanguageAgnosticTests/bin/Debug/net8.0/ bin
COPY --from=build /project/tests/LanguageAgnosticTests/*.py .
COPY --from=build /project/tests/LanguageAgnosticTests/*.docopt .

RUN apk add --no-cache python3

CMD [ "python3" , "language_agnostic_tester.py", "bin/Testee" ]
