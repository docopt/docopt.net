FROM mcr.microsoft.com/dotnet/sdk:6.0.201-alpine3.15-amd64 AS build

WORKDIR /project

COPY Directory.* .
COPY src/DocoptNet src/DocoptNet
COPY tests/LanguageAgnosticTests tests/LanguageAgnosticTests

RUN dotnet build tests/LanguageAgnosticTests

FROM mcr.microsoft.com/dotnet/runtime:6.0.3-alpine3.15-amd64

WORKDIR /tests

COPY --from=build /project/tests/LanguageAgnosticTests/bin/Debug/net6.0/ bin
COPY --from=build /project/tests/LanguageAgnosticTests/*.py .
COPY --from=build /project/tests/LanguageAgnosticTests/*.docopt .

RUN apk add --no-cache python3

CMD [ "python3" , "language_agnostic_tester.py", "bin/Testee" ]
