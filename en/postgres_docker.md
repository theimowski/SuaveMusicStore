# Postgres Docker

First we'll need to create a docker image for our database.

> Note: If you're not familiar with Docker, and have [pluralsight](https://www.pluralsight.com/) subscription, I highly recommend watching [this](https://www.pluralsight.com/courses/docker-deep-dive) docker deep-dive course by [Nigel Poulton](https://twitter.com/nigelpoulton). This is a really great course and it gives a quick ramp up on docker, just enough to get started. There is also a plenty of other learning docker resources available. Anyway even without basic docker knowledge, you should be able to proceed with the tutorial.

Let's create a separate `postgres` directory inside project's root folder to keep track of database-related files.
Now we can download [`postgres_create.sql` script](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/src_v{{book.version}}/postgres/postgres_create.sql) and save it under newly created `postgres` directory.

The db image will be based on the official `postgres` Docker image. On top of that, we'll run our `postgres_create.sql` script. This can be declared in the lines of following Dockerfile (create `Dockerfile` under `postgres` directory):

```Dockerfile
FROM postgres

COPY postgres_create.sql /docker-entrypoint-initdb.d/postgres_create.sql
```

The `COPY` instruction will place the script in a special directory inside the container, from which all scripts are run when the image is built.

With the script and Dockerfile in place, we can proceed to building the Docker image for database.
Run following command from the root directory:

```
> docker build -t theimowski/suavemusicstore_db:0.1 postgres
```

The command will build database image with a `theimowski/suavemusistore_db` tag in version `0.1` using Dockerfile from `postgres` directory.

After running the script and typing `docker images`, you should spot the newly built image:

```
REPOSITORY                      TAG                 IMAGE ID            CREATED             SIZE
theimowski/suavemusicstore_db   0.1                 45ac0e98f557        4 days ago          266 MB
postgres                        latest              ecd991538a0f        2 weeks ago         265 MB
```

To spin up a DB container from the image, simply type:

```
> docker run --name suavemusicstore_db -e POSTGRES_PASSWORD=mysecretpassword `
             -d -p 5432:5432 theimowski/suavemusicstore_db:0.1
```

Arguments to the above command are:

* proper name of the container (`--name <name>`),
* environment variable for Postgres password (`-e <VAR>=<val>`),
* `-d` switch to indicate detached (background) mode,
* mapped standard Postgres port 5432 from the container to host (`-p <hostPort>:<containerPort>`),
* the tag of image (`theimowski/suavemusicstore_db:0.1`).

If everything went fine, we should be able to see a running container with the `docker ps` command:

```
CONTAINER ID        IMAGE                               COMMAND                  CREATED             STATUS              PORTS                    NAMES
a47a4917d6af        theimowski/suavemusicstore_db:0.1   "docker-entrypoint..."   47 hours ago        Up 4 seconds        0.0.0.0:5432->5432/tcp   suavemusicstore_db
```
