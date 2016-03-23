#!/usr/bin/env bash

docker build -t theimowski/suavemusicstore_db:0.1 postgres
./build.sh
docker build -t theimowski/suavemusicstore_app:0.1 .
