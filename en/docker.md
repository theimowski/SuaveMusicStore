# Docker

We already have the database running in Docker, why don't we do the same with the web app?

Let's briefly go through the overall architecture of Suave Music Store running on Docker (the database part is already configured):

* We'll use two separate docker images:
  * First image for the database,
  * Second image for the actual F# app,
* The db image will be build on top of the [official postgres image](https://hub.docker.com/_/postgres/),
* The db image, upon build initialization, will create our `suavemusicstore` database from script,
* The app image will extend the [official fsharp image](https://hub.docker.com/_/fsharp/),
* The app image will copy **compiled** binaries to the image (more on that in later course),
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

At the moment starting web server (last line of App module) uses `defaultConfig`, which in turn uses binding to `127.0.0.1:8083` by default.
From [this](http://stackoverflow.com/a/27818259) Stack Overflow answer we can read that *"binding inside container to localhost usually prevent from accepting connections"*.
Solution here is to accept requests from all IPs instead.
This can be done by providing `0.0.0.0` address:

==> App.fs:`let cfg`

==> App.fs:`startWebServer`

The snippets above copies all fields from the `defaultConfig` and overrides the binding to `0.0.0.0:8083`.

## Server image

Database Docker image is already in place, under `postgres` directory.
The second image on the other hand will .
It will make use of the official fsharp image:

```bash
FROM fsharp/fsharp:latest

COPY ./bin/Debug /app

EXPOSE 8083

WORKDIR /app

CMD ["mono", "SuaveMusicStore.exe"]
```

It will use the `COPY` instruction as well, but this time we'll copy the whole directory with compiled binaries.
Because we're going to bind to port 8083, we need to declare that in the Dockerfile with the `EXPOSE` instruction.
`CMD` stands for command that is executed when we spin up a container from this image, and the preceding `WORKDIR` instruction simply states what the working directory should be when running all subsequent `RUN` all `CMD` instructions.

I deliberately chose to copy compiled binaries to the image instead of compiling the app inside docker, because in order to compile the Sql type by SQLProvider, an HTTP call needs to be invoked to read database schema.
As the database schema happens to be accessible on localhost, the easiest way was to compile locally, and only then copy binaries to the image.

## Building docker images

Following script (I called it `build_imgs.sh`) can help with building the actual docker images:

```bash
#!/usr/bin/env bash

docker build -t theimowski/suavemusicstore_db:0.1 postgres
./build.sh
docker build -t theimowski/suavemusicstore_app:0.1 .
```

It's very straightforward:

* build database image with a proper tag (`-t`) from directory `postgres` (Dockerfile for db image resides there)
* run our `build.sh` script to compile the application
* build app (server) image with a proper tag from current directory (`.`)

After running the script and typing `docker images`, you should spot the newly built images:

```bash
REPOSITORY                       TAG                 IMAGE ID            CREATED             SIZE
theimowski/suavemusicstore_app   0.1                 2fc4970e9b34        50 seconds ago      633.2 MB
theimowski/suavemusicstore_db    0.1                 143b21b4a88c        2 days ago          264.6 MB
mono                             4.2.3.4             81279c863851        7 days ago          628.7 MB
postgres                         latest              dbc8c4900ce5        8 days ago          264.6 MB
fsharp/fsharp                    latest              a91398194b54        4 months ago        730.2 MB
hello-world                      latest              690ed74de00f        5 months ago        960 B
```

## Spinning up containers

Now that we have the images in place, it's finally time to run the containers based on these images.
To do so, we can help ourselves with by writing following script (I called it just `run.sh`):

```bash
#!/usr/bin/env bash

# set up variables
export DB_CNTNR_NAME=suavemusicstore_db
export DB_CNTNR_ID=`docker ps -aq -f name=$DB_CNTNR_NAME`

export APP_CNTNR_NAME=suavemusicstore_app
export APP_CNTNR_ID=`docker ps -aq -f name=$APP_CNTNR_NAME`

# stop and remove containers with the same name
if [ -n "$APP_CNTNR_ID" ]; then
docker stop $APP_CNTNR_ID
docker rm $APP_CNTNR_ID
fi

if [ -n "$DB_CNTNR_ID" ]; then
docker stop $DB_CNTNR_ID
docker rm $DB_CNTNR_ID
fi

# run db container
docker run \
--name $DB_CNTNR_NAME \
-e POSTGRES_PASSWORD=mysecretpassword \
-d theimowski/suavemusicstore_db:0.1

# wait for the postgres to init
docker inspect --format {% raw %}'{{ .NetworkSettings.IPAddress }}:5453'{% endraw %} $DB_CNTNR_NAME \
| xargs wget --retry-connrefused --tries=5 -q --waitretry=3 --spider

# run server container
docker run \
-p 8083:8083 -d \
--name $APP_CNTNR_NAME \
--link $DB_CNTNR_NAME:$DB_CNTNR_NAME \
theimowski/suavemusicstore_app:0.1
```

The script above consists of a few parts:

1. First part simply prepares variables for naming db and app containers, as well as ids of already existing containers (if any)
2. Second part stops and removes containers that were found in the first part
3. Third part runs the db container with proper name (`--name`), environment variable (`-e`), (`-d`) switch to indicate detached (background) mode and finally the tag of image (theimowski/suavemusicstore_db:0.1)
4. Fourth part waits for the postgres db to initialize (it can take a few seconds). For this part I've copy-pasted and adjusted to my scenario a proposed solution from [this Stack Overflow answer](http://stackoverflow.com/a/25558040).
5. Finally the last part runs the app container:
  * `-p 8083:8083` instructs to expose the 8083 port from the container to the docker host
  * `-d` stands for detached (as above)
  * `--name` assigns a friendly name to the running container
  * `--link $DB_CNTNR_NAME:$DB_CNTNR_NAME` lets the app container communicate with the db container using the name of db container. Note the alias for the link ($DB_CNTNR_NAME) must be the same as in the Db module for the `getContext` function (in this case "suavemusicstore_db")
  * last argument is tag of the image

If everything went fine, we should now be able to see two running containers with the `docker ps` command:

```bash
CONTAINER ID        IMAGE                                COMMAND                  CREATED             STATUS              PORTS                    NAMES
3eb8ba5ec672        theimowski/suavemusicstore_app:0.1   "mono SuaveMusicStore"   43 seconds ago      Up 43 seconds       0.0.0.0:8083->8083/tcp   suavemusicstore_app
28abd8d491d8        theimowski/suavemusicstore_db:0.1    "/docker-entrypoint.s"   53 seconds ago      Up 52 seconds       5432/tcp                 suavemusicstore_db

```

Phew! We did it, Suave Music Store which originally was working on Windows only, now can be run on docker on a linux box.
How cool is that?
In case you encountered some problems - please let me know by creating a corresponding issue on [GitHub](https://github.com/theimowski/SuaveMusicStore), maybe we can sort things out.

State of the application ready to be run on docker: [Tag - docker](https://github.com/theimowski/SuaveMusicStore/tree/docker)