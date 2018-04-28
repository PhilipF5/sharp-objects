# Sharp Objects in a Bouncy Castle
A simple implementation of OpenPGP encryption in .NET Core using the Bouncy Castle project's OpenPGP API.

## Video Tutorial
**[Watch it live](https://www.unboxedtechnology.com/dev-talks/may-2018/) at noon (ET) on May 8, 2018.** Video will also be available to watch on demand afterward.

## How to Use This Repo
The `master` branch has the complete, fully functional code for this project. Checkout the `start` branch to get a mostly empty code template like the one we start with in the tutorial. Most of the shell commands in this README assume that you have cloned this repo and navigated into its root folder.

## Environment Setup — Pre-Built
If you don't want to spend time installing tools on your system, creating keys, or any of that, but you have Docker installed on your machine, you can spin up a quick, throwaway dev environment for the demo. The sharp-objects Docker image comes pre-built with all prerequisites and a sample PGP keypair for testing. This approach is fully cross-platform, with the caveats that Linux users need to [install Docker Compose separately](https://docs.docker.com/compose/install/), and Windows users need to be in Linux containers mode with their project drive shared in Docker settings. If you meet those conditions, getting yourself in business is simple:
```sh
docker-compose run --rm sharp-objects
```

That will pull the Docker image for you, mount your local files into a container so your local repo will be reflected, and launch a bash session.

**IMPORTANT:** The Docker image contains a sample PGP keypair created specifically for this demo. Because both the public and private keys come with the Docker image, this keypair is NOT SAFE TO USE FOR ANY ACTUAL SECURITY NEED. In the real world, NEVER make a private key publicly available.

## Environment Setup — DIY
If you want to build a persistent dev environment yourself, here are the tools you'll want to start things off. I'm not including developer basics like a good terminal, text editor, sample files, et cetera.

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

### Creating and Exporting PGP Keys
You'll need GNU Privacy Guard to create a PGP keypair. Run the following command to begin:
```sh
gpg --full-generate-key
```

The program will walk you through the key creation. In general, you should pick `4096` as your keysize. The key expiry/validity option is a choice I'm not covering in detail, but suffice it to say that `0` (no expiry) is perfectly fine for a keypair you're only using for academic purposes. At the end of the flow, you'll be shown an output with a description of the key, including a hexadecimal fingerprint you'll need for the next step.

#### Exporting
To read your keys with `FileStream` for Bouncy Castle, we'll need to export them. Grab that hexadecimal fingerprint from the last output, and use it in the following two commands. The second one will require your passphrase.
```sh
$ gpg --armor --output public.key --export <fingerprint>
$ gpg --armor --output private.key --export-secret-keys <fingerprint>
```

The `--armor` flag lets us export our keys in plaintext base64, rather than the default binary output. This is the format we typically use when working with PGP keys. You should now have the public and private key files in your project directory.

## Appendix
### New Project Setup
These are the commands I ran to initialize this project. Obviously I'm not including universal basics like `mkdir` or `git init`.
```sh
$ dotnet new console
$ dotnet add package BouncyCastle.OpenPGP --version 1.8.1.1
```

### Featured Tools
See a developer tool in the video that you want to try? Here's a list of tools and extensions I'm using in the presentation. Credit their authors for creating awesome stuff.
- [Fish shell](https://fishshell.com)
  - [Oh My Fish](https://github.com/oh-my-fish/oh-my-fish)
  - [bobthefish](https://github.com/oh-my-fish/theme-bobthefish)
  - [Meslo Regular Nerd Font Complete Mono](https://github.com/ryanoasis/nerd-fonts/tree/master/patched-fonts/Meslo)
- [Visual Studio Code](https://code.visualstudio.com)
  - [Bracket Pair Colorizer](https://marketplace.visualstudio.com/items?itemName=CoenraadS.bracket-pair-colorizer)
  - [Code Fragments](https://marketplace.visualstudio.com/items?itemName=markvincze.code-fragments)
  - [GitLens](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens)
  - [indent-rainbow](https://marketplace.visualstudio.com/items?itemName=oderwat.indent-rainbow)
  - [Monokai One Dark Vivid](https://marketplace.visualstudio.com/items?itemName=ashpowell.monokai-one-dark-vivid)
  - [vscode-icons](https://marketplace.visualstudio.com/items?itemName=robertohuertasm.vscode-icons)
