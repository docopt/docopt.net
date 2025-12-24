ARG SDK_VERSION=10.0.101
ARG RUNTIME_VERSION=10.0.1
ARG BASE_IMAGE=alpine3.22-amd64

FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION}-${BASE_IMAGE} AS build

WORKDIR /project

COPY Directory.* .
COPY src/DocoptNet src/DocoptNet
COPY tests/LanguageAgnosticTests tests/LanguageAgnosticTests

RUN dotnet build tests/LanguageAgnosticTests

FROM mcr.microsoft.com/dotnet/runtime:${RUNTIME_VERSION}-${BASE_IMAGE}

WORKDIR /tests

COPY --from=build /project/tests/LanguageAgnosticTests/bin/Debug/net10.0/ bin
COPY --from=build /project/tests/LanguageAgnosticTests/*.py .
COPY --from=build /project/tests/LanguageAgnosticTests/*.docopt .

RUN apk add --no-cache python3

CMD [ "python3" , "language_agnostic_tester.py", "bin/Testee" ]
