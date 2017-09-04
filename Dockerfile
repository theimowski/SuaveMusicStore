FROM fsharp:4.0

COPY ./bin/Debug/net461 /app

EXPOSE 8083

WORKDIR /app

CMD ["mono", "SuaveMusicStore.exe"]