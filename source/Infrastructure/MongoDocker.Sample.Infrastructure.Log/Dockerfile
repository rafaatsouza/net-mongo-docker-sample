ARG LOG_TOKEN=$LOG_TOKEN

FROM datalust/seqcli:5.1.251

RUN apt-get update && apt-get install -y curl

COPY ./docker-entrypoint.sh /

RUN ["chmod", "+x", "/docker-entrypoint.sh"]

ENTRYPOINT ["/docker-entrypoint.sh", "-c", $LOG_TOKEN]
