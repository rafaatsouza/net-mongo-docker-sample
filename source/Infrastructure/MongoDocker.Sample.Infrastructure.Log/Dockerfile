FROM datalust/seqcli:5.1.251

RUN apt-get update && apt-get install -y curl

COPY . .

RUN ["chmod", "+x", "docker-entrypoint.sh"]

ENTRYPOINT ["/bin/sh", "docker-entrypoint.sh"]