#!/usr/bin/env bash

export DB_CNTNR_NAME=suavemusicstore_db
export DB_CNTNR_ID=`docker ps -aq -f name=$DB_CNTNR_NAME`

export APP_CNTNR_NAME=suavemusicstore_app
export APP_CNTNR_ID=`docker ps -aq -f name=$APP_CNTNR_NAME`

if [ -n "$APP_CNTNR_ID" ]; then
	docker stop $APP_CNTNR_ID
	docker rm $APP_CNTNR_ID
fi

if [ -n "$DB_CNTNR_ID" ]; then
	docker stop $DB_CNTNR_ID
	docker rm $DB_CNTNR_ID
fi

docker run \
	--name $DB_CNTNR_NAME \
	-e POSTGRES_PASSWORD=mysecretpassword \
	-d theimowski/suavemusicstore_db:0.1

# wait for the postgres to init
docker inspect --format '{{ .NetworkSettings.IPAddress }}:5453' $DB_CNTNR_NAME \
	| xargs wget --retry-connrefused --tries=5 -q --waitretry=3 --spider

docker run \
	-p 8083:8083 -d \
	--name $APP_CNTNR_NAME \
	--link $DB_CNTNR_NAME:$DB_CNTNR_NAME \
	theimowski/suavemusicstore_app:0.1