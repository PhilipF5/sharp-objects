FROM microsoft/dotnet:2.0-sdk
RUN apt-get update && \
	apt-get install gnupg
RUN mkdir /opt/sharp-objects
WORKDIR /opt/sharp-objects
COPY private.key private.key
COPY public.key public.key
RUN gpg --list-keys
RUN gpg --batch --import private.key
