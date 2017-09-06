# Docker

We already have the database running in Docker, why don't we do the same with the web app?

Let's briefly go through the overall architecture of Suave Music Store running on Docker (the database part is already configured):

* We'll use two separate docker images:
  * First image for the database,
  * Second image for the actual F# app,
* The db image will be build on top of the [official postgres image](https://hub.docker.com/_/postgres/),
* The db image, upon build initialization, will create our `suavemusicstore` database from script,
* The app image will extend the [official fsharp image](https://hub.docker.com/_/fsharp/),
* The app image will copy compiled binaries to the image,
* The app image will depend on the db container, that's why we'll provide a link between those.

That's the big picture, let's now go straight to essential configuration:

## Database connection string

To run on docker, we'll have to provide a proper connection string.
Up until now, function `getContext` in Db module utilized the same connection string that was used for type provider.

But as we'll run the app container in isolation, we have to specify a proper connection string:

==> Db.fs:`let DockerConnectionString = `

==> Db.fs:`let getContext()`

Server `suavemusicstore_db` is the docker link name that we'll apply when firing up containers.
Docker networking infrastructure takes care of matching the link name with destination host, which will be run in a separate container.

**In real world** (yeah, I knew I'd use this phrase one day) we'd probably move the connection strings to some kind of configuration file.

## Server http binding

At the moment when starting web server (last line of App module), the `defaultConfig` is used, which in turn uses HTTP binding to `127.0.0.1` (localhost) IP address.
From [this](http://stackoverflow.com/a/27818259) Stack Overflow answer we can read that *"binding inside container to **localhost** usually prevent from accepting connections"*.
Solution here is to accept requests from all IPs instead.
This can be done by providing `0.0.0.0` address:

==> App.fs:`let cfg`

==> App.fs:`startWebServer`

The snippets above copies all fields from the `defaultConfig` and overrides the binding to `0.0.0.0:8083`.

## Server image

Database Docker image is already in place, under `postgres` directory.
It will make use of the official fsharp image:

```bash
FROM fsharp:4.0

COPY ./build /app

EXPOSE 8083

WORKDIR /app

CMD ["mono", "SuaveMusicStore.exe"]
```

It will use the `COPY` instruction as well, but this time we'll copy the whole directory with compiled binaries.
Because we're going to bind to port 8083, we need to declare that in the Dockerfile with the `EXPOSE` instruction.
`CMD` stands for command that is executed when we spin up a container from this image, and the preceding `WORKDIR` instruction simply states what the working directory should be when running all subsequent `RUN` all `CMD` instructions.

I chose to copy compiled binaries to the image instead of compiling the app inside docker. 
If desired however, one should be able to do the opposite and compile sources within the F# image itself.

## Building docker images

To build the docker image, we can invoke following commands:

```bash
> .\build.cmd
> docker build -t theimowski/suavemusicstore_app:0.1 .
```

Above snippet:

* runs our `build.cmd` script (`build.sh` for Mac and Linux) to compile the application
* builds app (server) image with a proper tag from current directory (`.`)

> Note: make sure you have compiled the app before building the image. 

After running the commands and typing `docker images`, you should spot the newly built image next to previous one:

```bash
REPOSITORY                       TAG     IMAGE ID            CREATED             SIZE
theimowski/suavemusicstore_app   0.1     2fc4970e9b34        50 seconds ago      633.2 MB
theimowski/suavemusicstore_db    0.1     143b21b4a88c        2 days ago          264.6 MB
```

## Spinning up web container

Now that we have the second image in place, it's finally time to run a container.
To do so, we can invoke following command:

```
> docker run -p 8083:8083 -d --name suavemusicstore_app `
  --link suavemusicstore_db:suavemusicstore_db theimowski/suavemusicstore_app:0.1
```

The command above consists of a few arguments:

* `-p 8083:8083` instructs to expose the 8083 port from the container to the docker host,
* `-d` stands for detached (background) mode,
* `--name` assigns a friendly name to the running container,
* `--link suavemusicstore_db:suavemusicstore_db` lets the app container communicate with the db container using the name of db container. Note the alias for the link (`suavemusicstore_db`) must be the same as in the Db module for the `getContext` function,
* last argument is the tag of the image.


If everything went fine, we should now be able to see two running containers with the `docker ps` command:

```bash
CONTAINER ID        IMAGE                                COMMAND                  CREATED             STATUS              PORTS                    NAMES
3eb8ba5ec672        theimowski/suavemusicstore_app:0.1   "mono SuaveMusicStore"   43 seconds ago      Up 43 seconds       0.0.0.0:8083->8083/tcp   suavemusicstore_app
28abd8d491d8        theimowski/suavemusicstore_db:0.1    "/docker-entrypoint.s"   53 seconds ago      Up 52 seconds       5432/tcp                 suavemusicstore_db

```

To try out the application, open up your browser and navigate to the endpoint on docker host:

* When running Docker Toolbox on Win / Mac, check out `docker-machine ip <docker VM name>` for docker host IP, and the endpoint should be something like `http://192.168.99.100:8083/`
* When running Docker natively on Linux, the endpoint should be just `http://localhost:8083/`

Phew! We did it, Suave Music Store is now running fully on Docker - How cool is that?