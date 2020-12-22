FROM microsoft/dotnet:2.0.3-runtime-jessie

MAINTAINER Devis Lucato (https://github.com/dluc)

LABEL Tags="Azure,IoT,Solutions,IoT Hub,Storage,.NET"

ARG user=pcsuser

RUN useradd -m -s /bin/bash -U $user

ENTRYPOINT ["/bin/bash", "/app/run.sh"]

COPY . /app/
RUN chown -R $user.$user /app
WORKDIR /app

USER $user
