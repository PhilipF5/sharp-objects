# Sharp Objects in a Bouncy Castle
A simple implementation of OpenPGP encryption in .NET Core using the Bouncy Castle project's OpenPGP API.

## Video Tutorial
**[Watch it live](https://www.unboxedtechnology.com/dev-talks/may-2018/) at noon (ET) on May 8, 2018. Video will also be available to watch on demand afterward.**

## Prerequisites
Here are the tools you'll want to start things off. I'm not including developer basics like a good terminal, text editor, sample files, et cetera.

*In this project, GNU Privacy Guard is used only for testing. If you have another OpenPGP toolkit you would rather test with, feel free to do so with the awareness that testing/verification will likely work differently than I've described.*

### macOS
For macOS, you'll need to install the .NET Core SDK and GNU Privacy Guard. Both are available via Homebrew.
```sh
$ brew cask install dotnet-sdk
$ brew install gnupg
```

### Linux
Distros based on Fedora or Debian (including Ubuntu and Linux Mint) typically come with GNU Privacy Guard pre-installed. For the .NET Core SDK, you can find distro-specific instructions on [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites).

### Windows
The .NET Core SDK is available from [Microsoft](https://www.microsoft.com/net/download/windows/build). Note that if you're using Visual Studio 2017, it only includes the .NET Core 1.x SDK by default, so if you want .NET Core 2.x you'll still need to download and install that separately. It will integrate with VS 2017 once you do.

The official version of GNU Privacy Guard for Windows is [Gpg4win](https://www.gpg4win.org). I haven't had occasion to use it, but it looks GUI-based so it shouldn't be too crazy to figure out.

## New Project Setup
These are the commands I ran to initialize this project. Obviously I'm not including universal basics like `mkdir` or `git init`.
```sh
$ dotnet new console
$ dotnet add package BouncyCastle.OpenPGP --version 1.8.1.1
```

