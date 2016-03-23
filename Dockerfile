FROM mono:4.2.3.4

COPY ./bin/Debug /app

EXPOSE 8083

WORKDIR /app

CMD ["mono", "SuaveMusicStore.exe"]
