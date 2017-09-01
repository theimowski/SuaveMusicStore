# Prerequisites

I did my best to make the tutorial cross platform applicable.
Although tested thoroughly **only** on Windows, you should be fine working with Linux / MacOS as well.
Be warned that CLI commands snippets are adjusted for PowerShell and thus are prompted with `>`.
Adjusting those snippets for *nix terminals should not be a big deal though.

Following is a list of required software to work with throughout the tutorial:

* [F#](http://fsharp.org) - you can find install instructions in this link, or follow instructions on Ionide site
* [Visual Studio Code](https://code.visualstudio.com/) - x-plat editor from Microsoft
* [Ionide](http://ionide.io/) - great F# extension for Visual Studio Code
* [Docker](https://www.docker.com) - container platform used for hosting a Postgres Database. As the Postgres image is Linux-based, you need to install [Docker Toolbox](https://www.docker.com/products/docker-toolbox) rather than [Docker Server](https://www.docker.com/community-edition), unless you're developing on Linux.