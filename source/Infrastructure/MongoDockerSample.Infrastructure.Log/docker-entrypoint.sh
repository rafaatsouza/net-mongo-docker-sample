#!/bin/sh -e

until curl -fs ${SEQ_SERVER_URL}  > /dev/null;
do
  >&2 echo "${SEQ_SERVER_URL} is unavailable - sleeping"
  sleep 2
done
>&2 echo "${SEQ_SERVER_URL} is up"

/bin/seqcli/seqcli apikey create --title='mongo-docker-api' --token=${SEQ_LOG_TOKEN} --server=${SEQ_SERVER_URL}

exit 0
